using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "discount",
                table: "Products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<string>(
                name: "summary",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "role_id", "description", "role_name" },
                values: new object[,]
                {
                    { "Admin", "", "Admin" },
                    { "Buyer", "", "Buyer" },
                    { "Seller", "", "Seller" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "role_id",
                keyValue: "Admin");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "role_id",
                keyValue: "Buyer");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "role_id",
                keyValue: "Seller");

            migrationBuilder.DropColumn(
                name: "summary",
                table: "Products");

            migrationBuilder.AlterColumn<double>(
                name: "discount",
                table: "Products",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
