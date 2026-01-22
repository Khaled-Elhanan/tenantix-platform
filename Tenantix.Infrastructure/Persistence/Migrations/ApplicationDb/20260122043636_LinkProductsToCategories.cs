using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tenantix.Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class LinkProductsToCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                schema: "Core",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId1",
                schema: "Core",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                schema: "Core",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId1",
                schema: "Core",
                table: "Products",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                schema: "Core",
                table: "Products",
                column: "CategoryId",
                principalSchema: "Core",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId1",
                schema: "Core",
                table: "Products",
                column: "CategoryId1",
                principalSchema: "Core",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                schema: "Core",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId1",
                schema: "Core",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                schema: "Core",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId1",
                schema: "Core",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "Core",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                schema: "Core",
                table: "Products");
        }
    }
}
