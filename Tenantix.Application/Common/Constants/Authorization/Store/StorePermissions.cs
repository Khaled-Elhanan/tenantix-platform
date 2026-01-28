namespace Tenantix.Application.Common.Constants.Authorization.Store;

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

        // ========= CATEGORIES =========
        new(StoreActions.Read, StoreFeatures.Categories, "View categories", "Catalog"),
        new(StoreActions.Create, StoreFeatures.Categories, "Create categories", "Catalog"),
        new(StoreActions.Update, StoreFeatures.Categories, "Update categories", "Catalog"),
        new(StoreActions.Delete, StoreFeatures.Categories, "Delete categories", "Catalog"),


        // ========= CUSTOMERS =========
        new(StoreActions.Read, StoreFeatures.Customers, "View customers", "Customers"),
        new(StoreActions.Create, StoreFeatures.Customers, "Create customers", "Customers"),
        new(StoreActions.Update, StoreFeatures.Customers, "Update customers", "Customers"),
        new(StoreActions.Delete, StoreFeatures.Customers, "Delete customers", "Customers"),

        // ========= ORDERS =========
        new(StoreActions.Read, StoreFeatures.Orders, "View orders", "Orders"),
        new(StoreActions.Update, StoreFeatures.Orders, "Update order status", "Orders"),
        new(StoreActions.Delete, StoreFeatures.Orders, "Delete orders", "Orders"),
        new(StoreActions.Create, StoreFeatures.Orders, "Create orders", "Orders"),

        // ========= CARTS =========
        new(StoreActions.Read, StoreFeatures.Carts, "View carts", "Orders"),
        new(StoreActions.Create, StoreFeatures.Carts, "Add items to cart", "Orders"),
        new(StoreActions.Update, StoreFeatures.Carts, "Update cart items", "Orders"),
        new(StoreActions.Delete, StoreFeatures.Carts, "Clear/remove cart items", "Orders"),


     

        // ========= BILLING =========
       // new(StoreActions.Manage, StoreFeatures.Billing, "Manage billing", "Billing", OwnerOnly: true),
       // new(StoreActions.Upgrade, StoreFeatures.Billing, "Upgrade subscription", "Billing", OwnerOnly: true),
    };

    public static IReadOnlyList<StorePermission> All => _all;

    public static IReadOnlyList<StorePermission> Owner =>
        _all;

    public static IReadOnlyList<StorePermission> Admin =>
        _all.Where(p => !p.OwnerOnly).ToList();

    public static IReadOnlyList<StorePermission> Customer =>
         _all.Where(p =>
        
        (p.Feature is StoreFeatures.Products or StoreFeatures.Categories && p.Action == StoreActions.Read)
     
        || (p.Feature is StoreFeatures.Carts &&
            (p.Action == StoreActions.Read ||
             p.Action == StoreActions.Create ||
             p.Action == StoreActions.Update ||
             p.Action == StoreActions.Delete))

    ).ToList();

    public static IReadOnlyList<StorePermission> Viewer =>
        _all.Where(p => p.Action == StoreActions.Read).ToList();

    public static string NameFor(string action, string feature) => $"Permission.{feature}.{action}";
}
