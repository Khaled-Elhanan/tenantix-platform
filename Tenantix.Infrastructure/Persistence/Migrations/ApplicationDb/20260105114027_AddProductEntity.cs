using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tenantix.Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddProductEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stores_Slug",
                schema: "Core",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "Core",
                table: "Stores");

            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrency",
                schema: "Core",
                table: "Stores",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "Core",
                table: "Stores",
                type: "uniqueidentifier",
                maxLength: 64,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "Core",
                table: "Stores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants ",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ValidUpTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants ", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stores_TenantId_Slug",
                schema: "Core",
                table: "Stores",
                columns: new[] { "TenantId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_StoreId",
                table: "Products",
                columns: new[] { "TenantId", "StoreId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants _Slug",
                schema: "Core",
                table: "Tenants ",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Tenants ",
                schema: "Core");

            migrationBuilder.DropIndex(
                name: "IX_Stores_TenantId_Slug",
                schema: "Core",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "DefaultCurrency",
                schema: "Core",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "Core",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "Core",
                table: "Stores");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "Core",
                table: "Stores",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EGY");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Slug",
                schema: "Core",
                table: "Stores",
                column: "Slug",
                unique: true);
        }
    }
}
