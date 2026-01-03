namespace Tenantix.Application.Common.Constants.Authorization;

public static class StorePermissions
{
    public record StorePermission(
        string Action,
        string Feature,
        string Description,
        string Group,
        bool OwnerOnly = false,
        bool RequiresPaidPlan = false)
    {
        public string Name => $"Permission.{Feature}.{Action}";

    }

    private static readonly StorePermission[] _all =
    {
        // ========= STORE =========
        new(StoreActions.Read, StoreFeatures.Store, "View store", "Store"),
        new(StoreActions.Update, StoreFeatures.Store, "Update store settings", "Store", OwnerOnly: true),

        // ========= PRODUCTS =========
        new(StoreActions.Read, StoreFeatures.Products, "View products", "Catalog"),
        new(StoreActions.Create, StoreFeatures.Products, "Create products", "Catalog"),
        new(StoreActions.Update, StoreFeatures.Products, "Update products", "Catalog"),
        new(StoreActions.Delete, StoreFeatures.Products, "Delete products", "Catalog"),

        // ========= ORDERS =========
        new(StoreActions.Read, StoreFeatures.Orders, "View orders", "Orders"),
        new(StoreActions.Update, StoreFeatures.Orders, "Update order status", "Orders"),

        // ========= STAFF =========
        new(StoreActions.Manage, StoreFeatures.Staff, "Manage staff", "Staff", OwnerOnly: true),

        // ========= BILLING =========
        new(StoreActions.Manage, StoreFeatures.Billing, "Manage billing", "Billing", OwnerOnly: true),
        new(StoreActions.Upgrade, StoreFeatures.Billing, "Upgrade subscription", "Billing", OwnerOnly: true),
    };

    public static IReadOnlyList<StorePermission> All => _all;

    public static IReadOnlyList<StorePermission> Owner =>
        _all;

    public static IReadOnlyList<StorePermission> Admin =>
        _all.Where(p => !p.OwnerOnly).ToList();

    public static IReadOnlyList<StorePermission> Staff =>
        _all.Where(p =>
            p.Feature is StoreFeatures.Products or StoreFeatures.Orders
        ).ToList();

    public static IReadOnlyList<StorePermission> Viewer =>
        _all.Where(p => p.Action == StoreActions.Read).ToList();

    public static string NameFor(string action, string feature) => $"Permission.{feature}.{action}";
}
