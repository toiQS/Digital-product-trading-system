using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Initcheckdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Orders_order_id",
                table: "Complaints");

            migrationBuilder.RenameColumn(
                name: "order_id",
                table: "Complaints",
                newName: "escrow_id");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_order_id",
                table: "Complaints",
                newName: "IX_Complaints_escrow_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Escrows_escrow_id",
                table: "Complaints",
                column: "escrow_id",
                principalTable: "Escrows",
                principalColumn: "escrow_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Escrows_escrow_id",
                table: "Complaints");

            migrationBuilder.RenameColumn(
                name: "escrow_id",
                table: "Complaints",
                newName: "order_id");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_escrow_id",
                table: "Complaints",
                newName: "IX_Complaints_order_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Orders_order_id",
                table: "Complaints",
                column: "order_id",
                principalTable: "Orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
