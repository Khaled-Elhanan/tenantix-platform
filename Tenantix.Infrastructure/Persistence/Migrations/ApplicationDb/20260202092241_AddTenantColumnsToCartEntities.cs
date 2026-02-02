using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tenantix.Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddTenantColumnsToCartEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId_ProductId",
                table: "CartItems");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                schema: "Core",
                table: "Products",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Carts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CartItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CartItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_TenantId_CustomerId",
                table: "Carts",
                columns: new[] { "TenantId", "CustomerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_TenantId_IsActive",
                table: "Carts",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_TenantId_CartId_ProductId",
                table: "CartItems",
                columns: new[] { "TenantId", "CartId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_TenantId_IsActive",
                table: "CartItems",
                columns: new[] { "TenantId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_TenantId_CustomerId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_TenantId_IsActive",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_TenantId_CartId_ProductId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_TenantId_IsActive",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                schema: "Core",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_ProductId",
                table: "CartItems",
                columns: new[] { "CartId", "ProductId" },
                unique: true);
        }
    }
}
