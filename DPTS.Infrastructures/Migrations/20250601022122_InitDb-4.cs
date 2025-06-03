using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class InitDb4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_logs_Users_user_id",
                table: "logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_logs",
                table: "logs");

            migrationBuilder.RenameTable(
                name: "logs",
                newName: "Logs");

            migrationBuilder.RenameIndex(
                name: "IX_logs_user_id",
                table: "Logs",
                newName: "IX_Logs_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logs",
                table: "Logs",
                column: "log_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_user_id",
                table: "Logs",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_user_id",
                table: "Logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Logs",
                table: "Logs");

            migrationBuilder.RenameTable(
                name: "Logs",
                newName: "logs");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_user_id",
                table: "logs",
                newName: "IX_logs_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_logs",
                table: "logs",
                column: "log_id");

            migrationBuilder.AddForeignKey(
                name: "FK_logs_Users_user_id",
                table: "logs",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id");
        }
    }
}
