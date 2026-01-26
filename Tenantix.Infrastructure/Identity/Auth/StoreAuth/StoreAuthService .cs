using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tenantix.Application.Common.Identity.Tokens;
using Tenantix.Application.Common.Interfaces;
using Tenantix.Application.Features.StoreAuth.DTOs;
using Tenantix.Domain.Entities;
using Tenantix.Infrastructure.Identity.Models;
using Tenantix.Infrastructure.MultiTenancy.Models;
using Tenantix.Infrastructure.Persistence.Context;
using Tenantix.Shared.Exceptions;

namespace Tenantix.Infrastructure.Identity.Auth.StoreAuth
{
    public class StoreAuthService : IStoreAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMultiTenantContextAccessor<ApplicationTenantInfo> _tenantAccessor;

        public StoreAuthService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ITokenService tokenService,
            IMultiTenantContextAccessor<ApplicationTenantInfo> tenantAccessor)
        {
            _userManager = userManager;
            _context = context;
            _tokenService = tokenService;
            _tenantAccessor = tenantAccessor;
        }
        private static string BuildTenantAwareName(string tenantId, string value)
                => $"{tenantId}__{value}";

      
        // REGISTER CUSTOMER
        public async Task<TokenResponse> RegisterAsync(
            StoreRegisterRequest request,
            CancellationToken cancellationToken)
        {
            var tenant = _tenantAccessor.MultiTenantContext?.TenantInfo
                ?? throw new UnauthorizedException(new() { "Tenant context not found." });

            if (!tenant.IsActive)
                throw new UnauthorizedException(new() { "Tenant is not active. Contact Administrator." });

            var email = request.Email.Trim().ToLowerInvariant();

            // Check if Identity user already exists in this tenant
            var existingUser = await _userManager.Users
                .AnyAsync(u =>
                    u.TenantId == tenant.Identifier &&
                    u.Email != null &&
                    u.Email.ToLower() == email,
                    cancellationToken);

            if (existingUser)
                throw new ConflictException(new() { "Email already registered." });

           
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
                TenantId = tenant.Identifier,
                IsActive = true
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
                throw new IdentityException(
                    createResult.Errors.Select(e => e.Description).ToList());

            // Assign Customer role
            var customerRole = BuildTenantAwareName(tenant.Identifier, "Customer");
            await _userManager.AddToRoleAsync(user, customerRole);


            // Link or create Customer profile (tenant filter applied globally)
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email, cancellationToken);

            if (customer is null)
            {
                customer = new Customer
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = email,
                    Phone = request.Phone,
                    UserId = user.Id
                };
                _context.Customers.Add(customer);
            }
            else
            {
                // Admin-created customer → link account
                customer.UserId = user.Id;
                customer.FirstName = request.FirstName;
                customer.LastName = request.LastName;
                customer.Phone = request.Phone;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Issue token using EXISTING login flow
            return await _tokenService.LoginAsync(new TokenRequest
            {
                Username = email,
                Password = request.Password
            });

        }
        // LOGIN CUSTOMER
        public async Task<TokenResponse> LoginAsync(
            StoreLoginRequest request,
            CancellationToken cancellationToken)
        {
            var tenant = _tenantAccessor.MultiTenantContext?.TenantInfo
                ?? throw new UnauthorizedException(new() { "Tenant context not found." });

            if (!tenant.IsActive)
                throw new UnauthorizedException(new() { "Tenant is not active. Contact Administrator." });

            // Authenticate using existing TokenService
            var token = await _tokenService.LoginAsync(new TokenRequest
            {
                Username = request.Email.Trim(),
                Password = request.Password
            });

            // ensure user --> customer 
            var user = await _userManager.FindByNameAsync(request.Email.Trim())
                       ?? await _userManager.FindByEmailAsync(request.Email.Trim());

            if (user is null)
                throw new UnauthorizedException(new() { "Authentication not successful." });

            if (user.TenantId != tenant.Identifier)
                throw new UnauthorizedException(new() { "User does not belong to this tenant." });

            var roles = await _userManager.GetRolesAsync(user);
            var customerRole = BuildTenantAwareName(tenant.Identifier, "Customer");
            if (!roles.Contains(customerRole))
                throw new ForbiddenException(new() { "This account is not a customer account." });

            return token;
        }
        public async Task ForgotPasswordAsync(string email, CancellationToken cancellationToken)
        {
            var tenant = _tenantAccessor.MultiTenantContext?.TenantInfo
                ?? throw new UnauthorizedException(new() { "Tenant context not found." });

            if (!tenant.IsActive)
                throw new UnauthorizedException(new() { "Tenant is not active. Contact Administrator." });

            var normalized = email.Trim();

            // Find user inside tenant (same rule as TokenService)
            var user = await _userManager.FindByNameAsync(normalized)
                   ?? await _userManager.FindByEmailAsync(normalized);

            // do not reveal existence
            if (user == null || user.TenantId != tenant.Identifier || !user.IsActive)
                return;

            // For ensure it's a customer account 
            var roles = await _userManager.GetRolesAsync(user);
            var customerRole = BuildTenantAwareName(tenant.Identifier, "Customer");
            if (!roles.Contains(customerRole))
                return;

            // Generate token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken)
        {
            var tenant = _tenantAccessor.MultiTenantContext?.TenantInfo
                ?? throw new UnauthorizedException(new() { "Tenant context not found." });

            if (!tenant.IsActive)
                throw new UnauthorizedException(new() { "Tenant is not active. Contact Administrator." });

            var normalized = email.Trim();

            var user = await _userManager.FindByNameAsync(normalized)
                   ?? await _userManager.FindByEmailAsync(normalized);

            if (user == null || user.TenantId != tenant.Identifier)
                throw new UnauthorizedException(new() { "Invalid reset request." });

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
                throw new IdentityException(result.Errors.Select(e => e.Description).ToList());
        }

   
    }
}
