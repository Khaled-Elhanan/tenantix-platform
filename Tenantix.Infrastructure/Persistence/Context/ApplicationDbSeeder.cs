using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Tenantix.Application.Common.Constants.Authorization;
using Tenantix.Application.Common.Constants.Tenancy;
using Tenantix.Infrastructure.Identity.Models;
using Tenantix.Infrastructure.MultiTenancy;
using Tenantix.Infrastructure.Persistence.Tenant;

public class ApplicationDbSeeder
{
    private readonly IMultiTenantContextAccessor<ApplicationTenantInfo> _tenantContext;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ApplicationDbSeeder(
        RoleManager<ApplicationRole> roleManager,
        IMultiTenantContextAccessor<ApplicationTenantInfo> tenantContext,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _roleManager = roleManager;
        _tenantContext = tenantContext;
        _userManager = userManager;
        _context = context;
    }

    public async Task InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        var pending = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pending.Any())
            await _context.Database.MigrateAsync(cancellationToken);

        await InitializeRolesAsync(cancellationToken);
        await InitializeOwnerUserAsync(cancellationToken);
    }

    // =========================================================
    // Roles
    // =========================================================
    private async Task InitializeRolesAsync(CancellationToken cancellationToken)
    {
        var tenant = _tenantContext.MultiTenantContext?.TenantInfo
            ?? throw new InvalidOperationException("Tenant context is missing.");

        var tenantId = tenant.Identifier!;

        var roles = new[]
        {
            ("Owner", StorePermissions.Owner),
            ("Admin", StorePermissions.Admin),
            ("Staff", StorePermissions.Staff),
            ("Viewer", StorePermissions.All.Where(p => p.Action == StoreActions.Read))
        };

        foreach (var (roleName, permissions) in roles)
        {
            var tenantRoleName = BuildTenantAwareName(tenantId, roleName);
            var role = await _roleManager.FindByNameAsync(tenantRoleName);

            if (role == null)
            {
                role = new ApplicationRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = tenantRoleName,
                    NormalizedName = tenantRoleName.ToUpperInvariant(),
                    Description = $"{roleName} role",
                    TenantId = tenantId
                };

                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                    throw new Exception(string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }

            await AssignPermissionsAsync(role, permissions.ToList(), cancellationToken);
        }
    }

    private async Task AssignPermissionsAsync(
        ApplicationRole role,
        IReadOnlyList<StorePermissions.StorePermission> permissions,
        CancellationToken cancellationToken)
    {
        var existingClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var permission in permissions)
        {
            if (existingClaims.Any(c =>
                    c.Type == ClaimConstants.Permissions &&
                    c.Value == permission.Name))
                continue;

            await _context.RoleClaims.AddAsync(new ApplicationRoleClaim
            {
                RoleId = role.Id,
                RoleName = role.Name,
                ClaimType = ClaimConstants.Permissions,
                ClaimValue = permission.Name,
                Description = permission.Description
            }, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    // =========================================================
    // Owner User
    // =========================================================
    private async Task InitializeOwnerUserAsync(CancellationToken cancellationToken)
    {
        var tenant = _tenantContext.MultiTenantContext?.TenantInfo;
        if (tenant == null) return;

        var tenantId = tenant.Identifier!;
        var email = tenant.OwnerEmail ?? $"{tenantId}@store.local";

        var username = Sanitize(BuildTenantAwareName(tenantId, email));

        var user = await _userManager.Users
            .SingleOrDefaultAsync(u =>
                u.UserName == username && u.TenantId == tenantId,
                cancellationToken);

        if (user == null)
        {
            user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                UserName = username,
                NormalizedEmail = email.ToUpperInvariant(),
                NormalizedUserName = username.ToUpperInvariant(),
                TenantId = tenantId,
                IsActive = true,
                EmailConfirmed = true
            };

            var hasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = hasher.HashPassword(
                user,
                TenancyConstants.Root.DefaultPassword);

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }

        var ownerRole = BuildTenantAwareName(tenantId, "Owner");
        if (!await _userManager.IsInRoleAsync(user, ownerRole))
            await _userManager.AddToRoleAsync(user, ownerRole);
    }

    // =========================================================
    // Helpers
    // =========================================================
    private static string BuildTenantAwareName(string tenantId, string value)
        => $"{tenantId}__{value}";

    private static string Sanitize(string input)
    {
        var lower = input.ToLowerInvariant();
        var clean = Regex.Replace(lower, "[^a-z0-9]", "_");
        return Regex.Replace(clean, "_{2,}", "_").Trim('_');
    }
}
