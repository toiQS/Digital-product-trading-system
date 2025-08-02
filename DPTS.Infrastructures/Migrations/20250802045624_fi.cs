using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class fi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdjustmentRules_Categories_CategoryId",
                table: "AdjustmentRules");

            migrationBuilder.DropIndex(
                name: "IX_AdjustmentRules_CategoryId",
                table: "AdjustmentRules");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "AdjustmentRules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                table: "AdjustmentRules",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdjustmentRules_CategoryId",
                table: "AdjustmentRules",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdjustmentRules_Categories_CategoryId",
                table: "AdjustmentRules",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "category_id");
        }
    }
}
