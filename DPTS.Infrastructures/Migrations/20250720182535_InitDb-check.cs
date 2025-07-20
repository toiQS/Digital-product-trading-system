using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class InitDbcheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Stores_StoreId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Stores_StoreId1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_StoreId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_StoreId1",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "StoreId1",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoreId",
                table: "Messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreId1",
                table: "Messages",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_StoreId",
                table: "Messages",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_StoreId1",
                table: "Messages",
                column: "StoreId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Stores_StoreId",
                table: "Messages",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "store_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Stores_StoreId1",
                table: "Messages",
                column: "StoreId1",
                principalTable: "Stores",
                principalColumn: "store_id");
        }
    }
}
