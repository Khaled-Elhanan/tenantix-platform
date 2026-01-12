using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tenantix.Application;
using Tenantix.Application.Common.Constants.MultiTenancy;
using Tenantix.Infrastructure.Identity.Models;
using Tenantix.Shared.Exceptions;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Common.Identity.Tokens;
using Tenantix.Application.Common.Identity;
using Tenantix.Infrastructure.MultiTenancy.Models;
using Tenantix.Application.Common.Constants.Authorization.Common;

namespace Tenantix.Infrastructure.Identity.Security;

public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMultiTenantContextAccessor<ApplicationTenantInfo> _tenantContextAccessor;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtSettings _jwtSettings;


        public TokenService(                           
            UserManager<ApplicationUser> userManager,
            IMultiTenantContextAccessor<ApplicationTenantInfo> tenantContextAccessor,
            RoleManager<ApplicationRole> roleManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _tenantContextAccessor = tenantContextAccessor;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<TokenResponse> LoginAsync(TokenRequest request)
        {
            #region Validations

            if (_tenantContextAccessor.MultiTenantContext?.TenantInfo == null)
            {
                throw new UnauthorizedException(new List<string> { "Tenant context not found." });
            }

            if (!_tenantContextAccessor.MultiTenantContext.TenantInfo.IsActive)
            {
                throw new UnauthorizedException(new List<string> { "Tenant is not active. Contact Administrator." });
            }

            // Try to find user by username first, then by email
            var userInDb = await _userManager.FindByNameAsync(request.Username);
            
            if (userInDb == null)
            {
                // Try finding by email if username lookup failed
                userInDb = await _userManager.FindByEmailAsync(request.Username);
            }

            if (userInDb == null)
            {
                throw new UnauthorizedException(new List<string> { "Authentication not successful. Invalid username/email or password." });
            }

            // Verify the user belongs to the current tenant
            if (userInDb.TenantId != _tenantContextAccessor.MultiTenantContext?.TenantInfo?.Identifier)
            {
                throw new UnauthorizedException(new List<string> { "Authentication not successful. User does not belong to this tenant." });
            }
          
            if (!await _userManager.CheckPasswordAsync(userInDb, request.Password))
            {
                throw new UnauthorizedException(new List<string> { "Incorrect username/email or password." });
            }

            if (!userInDb.IsActive)
            {
                throw new UnauthorizedException(new List<string> { "User is not active. Contact Administrator." });
            }

            // Check subscription for non-root tenants
            if (_tenantContextAccessor.MultiTenantContext.TenantInfo.Id is not TenancyConstants.Root.Id)
            {
                if (_tenantContextAccessor.MultiTenantContext.TenantInfo.ValidUpTo < DateTime.UtcNow)
                {
                    throw new UnauthorizedException(new List<string> { "Tenant Subscription has expired. Contact Administrator." });
                }
            }

            #endregion

            // Generate the Token JWT
            return await GenerateTokenAndUpdateUserAsync(userInDb);
        }


        // -------------------- Refresh Token ---------------------
        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            #region Validations

            if (_tenantContextAccessor.MultiTenantContext?.TenantInfo == null)
            {
                throw new UnauthorizedException(new List<string> { "Tenant context not found." });
            }

            if (!_tenantContextAccessor.MultiTenantContext.TenantInfo.IsActive)
            {
                throw new UnauthorizedException(new List<string> { "Tenant is not active. Contact Administrator." });
            }

            var userPrincipal = GetClaimsPrincipalFromExpiringToken(request.CurrentJwt);
            var userId = userPrincipal.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException(new List<string> { "Invalid token: user identifier not found." });
            }

            var userInDb = await _userManager.FindByIdAsync(userId) ??
                throw new UnauthorizedException(new List<string> { "Authentication failed. User not found." });

            if (userInDb.TenantId != _tenantContextAccessor.MultiTenantContext?.TenantInfo?.Identifier)
            {
                throw new UnauthorizedException(new List<string> { "Authentication failed. User does not belong to this tenant." });
            }

            if (!userInDb.IsActive)
            {
                throw new UnauthorizedException(new List<string> { "User is not active. Contact Administrator." });
            }

            // Validate refresh token: must match stored token and not be expired
            // This implements refresh token rotation - only the current refresh token is valid
            if (userInDb.RefreshToken != request.CurrentRefreshToken)
            {
                throw new UnauthorizedException(new List<string> { "Invalid refresh token. Token may have been rotated or revoked." });
            }

            if (userInDb.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new UnauthorizedException(new List<string> { "Refresh token has expired. Please login again." });
            }

            #endregion

            // Generate new tokens and rotate refresh token
            return await GenerateTokenAndUpdateUserAsync(userInDb);
        }

        private ClaimsPrincipal GetClaimsPrincipalFromExpiringToken(string expiringToken)
        {
            var tkValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = ClaimTypes.Role,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret))

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(expiringToken, tkValidationParams, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnauthorizedException(["Invalid token provided. Failed to generate new token."]);
            }
            return principal;
        }




        // ---------------- Generate Token & Update User ----------------
        private async Task<TokenResponse> GenerateTokenAndUpdateUserAsync(ApplicationUser user)
        {
            // Generate jwt
            var newJwt = await GenerateJwtToken(user);

            // Refresh Token (rotate)
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryTimeInDays);

            await _userManager.UpdateAsync(user);

            return new TokenResponse
            {
                jwt = newJwt,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryDate = user.RefreshTokenExpiryTime
            };
        }


        // ---------------- JWT Generator ----------------
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            // Generate the encrypted token
            return GenerateEncryptedToken(GenerateSigningCredentials(), await GetUserClaims(user));
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpiryTimeInMinutes),
                signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }


        // ---------------- Signing Key ----------------
        private SigningCredentials GenerateSigningCredentials()
        {
            byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }


        // ---------------- Claims ----------------
        private async Task<IEnumerable<Claim>> GetUserClaims(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();

            foreach (var userRole in userRoles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, userRole));
                var currentRole = await _roleManager.FindByNameAsync(userRole);
                var allPermissionForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
                permissionClaims.AddRange(allPermissionForCurrentRole);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.FirstName ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.LastName ?? string.Empty),
                new Claim(ClaimConstants.Tenant, _tenantContextAccessor.MultiTenantContext?.TenantInfo?.Id ?? string.Empty),
                new Claim(ClaimConstants.TenantType, _tenantContextAccessor.MultiTenantContext?.TenantInfo?.TenantType ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            }.Union(roleClaims)
            .Union(userClaims)
            .Union(permissionClaims);
            return claims;
        }


        // ---------------- Refresh Token Generator ----------------
        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

   
}

                                                     