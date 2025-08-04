using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Initdb19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "PaymentMethods",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "PaymentMethods");
        }
    }
}
