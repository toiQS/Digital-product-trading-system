using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class InitDb3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Orders_order_id",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_Users_user_id",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_user_id",
                table: "Logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Address_AddressId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Users_UserId",
                table: "WalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_trade_from_id",
                table: "WalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_trade_to_id",
                table: "WalletTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Logs",
                table: "Logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletTransactions",
                table: "WalletTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Complaints",
                table: "Complaints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.RenameTable(
                name: "Logs",
                newName: "logs");

            migrationBuilder.RenameTable(
                name: "WalletTransactions",
                newName: "Trades");

            migrationBuilder.RenameTable(
                name: "Complaints",
                newName: "Complications");

            migrationBuilder.RenameTable(
                name: "Address",
                newName: "Addresses");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_user_id",
                table: "logs",
                newName: "IX_logs_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransactions_UserId",
                table: "Trades",
                newName: "IX_Trades_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransactions_trade_to_id",
                table: "Trades",
                newName: "IX_Trades_trade_to_id");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransactions_trade_from_id",
                table: "Trades",
                newName: "IX_Trades_trade_from_id");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_user_id",
                table: "Complications",
                newName: "IX_Complications_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Complaints_order_id",
                table: "Complications",
                newName: "IX_Complications_order_id");

            migrationBuilder.AlterColumn<string>(
                name: "AddressId",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Trades",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_logs",
                table: "logs",
                column: "log_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trades",
                table: "Trades",
                column: "trade_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Complications",
                table: "Complications",
                column: "complaint_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "address_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Complications_Orders_order_id",
                table: "Complications",
                column: "order_id",
                principalTable: "Orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Complications_Users_user_id",
                table: "Complications",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_logs_Users_user_id",
                table: "logs",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Users_UserId",
                table: "Trades",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Wallets_trade_from_id",
                table: "Trades",
                column: "trade_from_id",
                principalTable: "Wallets",
                principalColumn: "wallet_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Wallets_trade_to_id",
                table: "Trades",
                column: "trade_to_id",
                principalTable: "Wallets",
                principalColumn: "wallet_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Addresses_AddressId",
                table: "Users",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "address_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complications_Orders_order_id",
                table: "Complications");

            migrationBuilder.DropForeignKey(
                name: "FK_Complications_Users_user_id",
                table: "Complications");

            migrationBuilder.DropForeignKey(
                name: "FK_logs_Users_user_id",
                table: "logs");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Users_UserId",
                table: "Trades");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Wallets_trade_from_id",
                table: "Trades");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Wallets_trade_to_id",
                table: "Trades");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Addresses_AddressId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_logs",
                table: "logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trades",
                table: "Trades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Complications",
                table: "Complications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "logs",
                newName: "Logs");

            migrationBuilder.RenameTable(
                name: "Trades",
                newName: "WalletTransactions");

            migrationBuilder.RenameTable(
                name: "Complications",
                newName: "Complaints");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "Address");

            migrationBuilder.RenameIndex(
                name: "IX_logs_user_id",
                table: "Logs",
                newName: "IX_Logs_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Trades_UserId",
                table: "WalletTransactions",
                newName: "IX_WalletTransactions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Trades_trade_to_id",
                table: "WalletTransactions",
                newName: "IX_WalletTransactions_trade_to_id");

            migrationBuilder.RenameIndex(
                name: "IX_Trades_trade_from_id",
                table: "WalletTransactions",
                newName: "IX_WalletTransactions_trade_from_id");

            migrationBuilder.RenameIndex(
                name: "IX_Complications_user_id",
                table: "Complaints",
                newName: "IX_Complaints_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Complications_order_id",
                table: "Complaints",
                newName: "IX_Complaints_order_id");

            migrationBuilder.AlterColumn<string>(
                name: "AddressId",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "WalletTransactions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logs",
                table: "Logs",
                column: "log_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletTransactions",
                table: "WalletTransactions",
                column: "trade_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Complaints",
                table: "Complaints",
                column: "complaint_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "address_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Orders_order_id",
                table: "Complaints",
                column: "order_id",
                principalTable: "Orders",
                principalColumn: "order_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_Users_user_id",
                table: "Complaints",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_user_id",
                table: "Logs",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Address_AddressId",
                table: "Users",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "address_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Users_UserId",
                table: "WalletTransactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_trade_from_id",
                table: "WalletTransactions",
                column: "trade_from_id",
                principalTable: "Wallets",
                principalColumn: "wallet_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_trade_to_id",
                table: "WalletTransactions",
                column: "trade_to_id",
                principalTable: "Wallets",
                principalColumn: "wallet_id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
