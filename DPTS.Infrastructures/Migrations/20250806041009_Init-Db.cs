using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "PaymentMethods");

            migrationBuilder.AddColumn<string>(
                name: "source_id",
                table: "AdjustmentRules",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "source_id",
                table: "AdjustmentRules");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "PaymentMethods",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
