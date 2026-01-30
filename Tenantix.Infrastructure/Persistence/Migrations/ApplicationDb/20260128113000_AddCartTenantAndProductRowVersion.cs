using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tenantix.Infrastructure.Migrations.ApplicationDb
{
    public class AddCartTenantAndProductRowVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- Carts / CartItems: make them tenant-scoped & soft-filterable (IsActive)
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CartItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CartItems",
                type: "bit",
                nullable: false,
                defaultValue: true);

            // Rebuild indexes to include TenantId
            migrationBuilder.DropIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_TenantId_CustomerId",
                table: "Carts",
                columns: new[] { "TenantId", "CustomerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_TenantId_IsActive",
                table: "Carts",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId_ProductId",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_TenantId_CartId_ProductId",
                table: "CartItems",
                columns: new[] { "TenantId", "CartId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_TenantId_IsActive",
                table: "CartItems",
                columns: new[] { "TenantId", "IsActive" });

            // --- Products: add optimistic concurrency token
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "Core",
                table: "Products",
                type: "rowversion",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "Core",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_TenantId_CartId_ProductId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_TenantId_IsActive",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_ProductId",
                table: "CartItems",
                columns: new[] { "CartId", "ProductId" },
                unique: true);

            migrationBuilder.DropIndex(
                name: "IX_Carts_TenantId_CustomerId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_TenantId_IsActive",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts",
                column: "CustomerId",
                unique: true);

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Carts");
        }
    }
}
