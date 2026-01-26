using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tenantix.Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class LinkCustomerToIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Email",
                schema: "Core",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "Core",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Core",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId_Email",
                schema: "Core",
                table: "Customers",
                columns: new[] { "TenantId", "Email" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_TenantId_Email",
                schema: "Core",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Core",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                schema: "Core",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                schema: "Core",
                table: "Customers",
                column: "Email");
        }
    }
}
