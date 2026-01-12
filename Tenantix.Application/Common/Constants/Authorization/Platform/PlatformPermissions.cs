using Tenantix.Application.Common.Constants.Authorization.Platform;

namespace Tenantix.Application.Common.Constants.Authorization;

public static class PlatformPermissions
{
    // Const names for attribute usage
    public const string TenantsRead = "Platform.Tenants.Read";
    public const string TenantsCreate = "Platform.Tenants.Create";
    public const string TenantsActivate = "Platform.Tenants.Activate";
    public const string TenantsDeactivate = "Platform.Tenants.Deactivate";
    public const string TenantsUpgrade = "Platform.Tenants.Upgrade";
    public const string TenantsUpdate = "Platform.Tenants.Update";

    public const string BillingRead = "Platform.Billing.Read";
    public const string BillingManage = "Platform.Billing.Manage";

    public const string SystemRead = "Platform.System.Read";
    public const string SystemManage = "Platform.System.Manage";

    public record PlatformPermission(
        string Action,
        string Feature,
        string Description,
        string Group,
        string Name,
        bool OwnerOnly = false);

    private static readonly PlatformPermission[] _all =
    {
        // Tenants
        new(PlatformActions.Read, PlatformFeatures.Tenants, "View tenants", "Tenants", TenantsRead),
        new(PlatformActions.Create, PlatformFeatures.Tenants, "Create tenants", "Tenants", TenantsCreate, OwnerOnly: true),
        new(PlatformActions.Activate, PlatformFeatures.Tenants, "Activate tenants", "Tenants", TenantsActivate, OwnerOnly: true),
        new(PlatformActions.Deactivate, PlatformFeatures.Tenants, "Deactivate tenants", "Tenants", TenantsDeactivate, OwnerOnly: true),
        new(PlatformActions.Upgrade, PlatformFeatures.Tenants, "Upgrade tenant subscription", "Tenants", TenantsUpgrade, OwnerOnly: true),
        new(PlatformActions.Update, PlatformFeatures.Tenants, "Update tenant settings", "Tenants", TenantsUpdate, OwnerOnly: true),

        // Billing
        new(PlatformActions.Read, PlatformFeatures.Billing, "View platform billing", "Billing", BillingRead),
        new(PlatformActions.Manage, PlatformFeatures.Billing, "Manage platform billing", "Billing", BillingManage, OwnerOnly: true),

        // System
        new(PlatformActions.Read, PlatformFeatures.System, "View system status", "System", SystemRead),
        new(PlatformActions.Manage, PlatformFeatures.System, "Manage platform settings", "System", SystemManage, OwnerOnly: true),
    };

    public static IReadOnlyList<PlatformPermission> All => _all;
    public static IReadOnlyList<PlatformPermission> Owner => _all;
    public static IReadOnlyList<PlatformPermission> Admin => _all.Where(p => !p.OwnerOnly).ToList();

    public static string NameFor(string action, string feature) => $"Platform.{feature}.{action}";
}

