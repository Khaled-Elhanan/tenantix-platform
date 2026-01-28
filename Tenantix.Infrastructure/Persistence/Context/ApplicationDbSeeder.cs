using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Tenantix.Application.Common.Constants.Authorization.Common;
using Tenantix.Application.Common.Constants.Authorization.Store;
using Tenantix.Application.Common.Constants.MultiTenancy;
using Tenantix.Infrastructure.Identity.Models;
using Tenantix.Infrastructure.MultiTenancy.Models;
using Tenantix.Infrastructure.Persistence.Context;

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
        await RunMigrationsAsync(cancellationToken);
        await InitializeRolesAsync(cancellationToken);
        await InitializeRoleClaimsAsync(cancellationToken);
        await InitializeAdminUserAsync(cancellationToken);
    }

    protected virtual async Task RunMigrationsAsync(CancellationToken cancellationToken)
    {
        var pending = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pending.Any())
            await _context.Database.MigrateAsync(cancellationToken);
    }

    private async Task InitializeRolesAsync(CancellationToken cancellationToken)
    {
        var tenant = _tenantContext.MultiTenantContext?.TenantInfo
            ?? throw new InvalidOperationException("Tenant context is missing.");

        var tenantId = tenant.Identifier
            ?? throw new InvalidOperationException("Tenant identifier is missing.");

        var roles = tenant.TenantType == TenancyConstants.TenantTypes.Platform
            ? new[] { "Admin" }
            : new[] { "Admin", "Customer" };

        foreach (var roleName in roles)
        {
            var tenantRoleName = BuildTenantAwareName(tenantId, roleName);

            var role = await _roleManager.FindByNameAsync(tenantRoleName);
            if (role != null)
                continue;

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
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private async Task InitializeRoleClaimsAsync(CancellationToken cancellationToken)
    {
        var tenant = _tenantContext.MultiTenantContext?.TenantInfo
            ?? throw new InvalidOperationException("Tenant context is missing.");

        var tenantId = tenant.Identifier
            ?? throw new InvalidOperationException("Tenant identifier is missing.");

        if (tenant.TenantType == TenancyConstants.TenantTypes.Platform)
            return;

        await SeedStoreRoleClaimsAsync(tenantId, "Admin", StorePermissions.Admin, cancellationToken);
        await SeedStoreRoleClaimsAsync(tenantId, "Customer", StorePermissions.Customer, cancellationToken);
    }

    private async Task SeedStoreRoleClaimsAsync(
        string tenantId,
        string roleBaseName,
        IReadOnlyList<StorePermissions.StorePermission> permissions,
        CancellationToken cancellationToken)
    {
        var roleName = BuildTenantAwareName(tenantId, roleBaseName);
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role is null) return;

        var existingValues = await _context.RoleClaims
            .AsNoTracking()
            .Where(rc => rc.RoleId == role.Id && rc.ClaimType == ClaimConstants.Permissions)
            .Select(rc => rc.ClaimValue)
            .ToListAsync(cancellationToken);

        foreach (var perm in permissions)
        {
            var permName = perm.Name;

            if (existingValues.Contains(permName))
                continue;

            _context.RoleClaims.Add(new ApplicationRoleClaim
            {
                RoleId = role.Id,
                RoleName = role.Name!,
                ClaimType = ClaimConstants.Permissions,
                ClaimValue = permName,
                Description = perm.Description
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task InitializeAdminUserAsync(CancellationToken cancellationToken)
    {
        var tenant = _tenantContext.MultiTenantContext?.TenantInfo;
        if (tenant == null) return;

        if (tenant.TenantType == TenancyConstants.TenantTypes.Platform)
            return;

        var tenantId = tenant.Identifier!;
        var email = tenant.OwnerEmail ?? $"{tenantId}@store.local";
        var username = Sanitize(BuildTenantAwareName(tenantId, email));

        var user = await _userManager.Users
            .SingleOrDefaultAsync(u => u.UserName == username && u.TenantId == tenantId, cancellationToken);

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
                EmailConfirmed = true,
                FirstName = tenantId,
                LastName = "Admin"
            };

            var hasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = hasher.HashPassword(user, TenancyConstants.Root.DefaultPassword);

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var adminRole = BuildTenantAwareName(tenantId, "Admin");
        if (!await _userManager.IsInRoleAsync(user, adminRole))
        {
            await _userManager.AddToRoleAsync(user, adminRole);
        }
    }

    private static string BuildTenantAwareName(string tenantId, string value)
        => $"{tenantId}__{value}";

    private static string Sanitize(string input)
    {
        var lower = input.ToLowerInvariant();
        var clean = Regex.Replace(lower, "[^a-z0-9]", "_");
        return Regex.Replace(clean, "_{2,}", "_").Trim('_');
    }
}
