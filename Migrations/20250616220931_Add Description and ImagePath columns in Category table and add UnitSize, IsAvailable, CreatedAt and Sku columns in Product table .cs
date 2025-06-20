using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionandImagePathcolumnsinCategorytableandaddUnitSizeIsAvailableCreatedAtandSkucolumnsinProducttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnitSize",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UnitSize",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Categories");
        }
    }
}
