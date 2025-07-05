using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class FinalCheckDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_user_id",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "base_price",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "discount_amount",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "final_price",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ip_address",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "user_agent",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "user_type",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "is_default",
                table: "AdjustmentRules");

            migrationBuilder.RenameColumn(
                name: "summary",
                table: "Products",
                newName: "summary feature");

            migrationBuilder.RenameColumn(
                name: "tax_amount",
                table: "OrderItems",
                newName: "total_price");

            migrationBuilder.RenameColumn(
                name: "platform_fee_amount",
                table: "OrderItems",
                newName: "price_for_each_product");

            migrationBuilder.RenameColumn(
                name: "action",
                table: "Logs",
                newName: "Action");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Logs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "target_type",
                table: "Logs",
                newName: "TargetType");

            migrationBuilder.RenameColumn(
                name: "target_id",
                table: "Logs",
                newName: "TargetId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Logs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "log_id",
                table: "Logs",
                newName: "LogId");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_user_id",
                table: "Logs",
                newName: "IX_Logs_UserId");

            migrationBuilder.RenameColumn(
                name: "version",
                table: "AdjustmentRules",
                newName: "target_logic");

            migrationBuilder.AddColumn<bool>(
                name: "email_verified",
                table: "UserSecurities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "actual_amount",
                table: "Escrows",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "released_at",
                table: "Escrows",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "released_by",
                table: "Escrows",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "tax_amount",
                table: "Escrows",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "tax_rate",
                table: "Escrows",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "conditions_json",
                table: "AdjustmentRules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "max_cap",
                table: "AdjustmentRules",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "min_order_amount",
                table: "AdjustmentRules",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "per_user_limit",
                table: "AdjustmentRules",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "scope",
                table: "AdjustmentRules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "source",
                table: "AdjustmentRules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "usage_limit",
                table: "AdjustmentRules",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "voucher_code",
                table: "AdjustmentRules",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_UserId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "email_verified",
                table: "UserSecurities");

            migrationBuilder.DropColumn(
                name: "actual_amount",
                table: "Escrows");

            migrationBuilder.DropColumn(
                name: "released_at",
                table: "Escrows");

            migrationBuilder.DropColumn(
                name: "released_by",
                table: "Escrows");

            migrationBuilder.DropColumn(
                name: "tax_amount",
                table: "Escrows");

            migrationBuilder.DropColumn(
                name: "tax_rate",
                table: "Escrows");

            migrationBuilder.DropColumn(
                name: "conditions_json",
                table: "AdjustmentRules");

            migrationBuilder.DropColumn(
                name: "max_cap",
                table: "AdjustmentRules");

            migrationBuilder.DropColumn(
                name: "min_order_amount",
                table: "AdjustmentRules");

            migrationBuilder.DropColumn(
                name: "per_user_limit",
                table: "AdjustmentRules");

            migrationBuilder.DropColumn(
                name: "scope",
                table: "AdjustmentRules");

            migrationBuilder.DropColumn(
                name: "source",
                table: "AdjustmentRules");

            migrationBuilder.DropColumn(
                name: "usage_limit",
                table: "AdjustmentRules");

            migrationBuilder.DropColumn(
                name: "voucher_code",
                table: "AdjustmentRules");

            migrationBuilder.RenameColumn(
                name: "summary feature",
                table: "Products",
                newName: "summary");

            migrationBuilder.RenameColumn(
                name: "total_price",
                table: "OrderItems",
                newName: "tax_amount");

            migrationBuilder.RenameColumn(
                name: "price_for_each_product",
                table: "OrderItems",
                newName: "platform_fee_amount");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "Logs",
                newName: "action");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Logs",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "TargetType",
                table: "Logs",
                newName: "target_type");

            migrationBuilder.RenameColumn(
                name: "TargetId",
                table: "Logs",
                newName: "target_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Logs",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "LogId",
                table: "Logs",
                newName: "log_id");

            migrationBuilder.RenameIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                newName: "IX_Logs_user_id");

            migrationBuilder.RenameColumn(
                name: "target_logic",
                table: "AdjustmentRules",
                newName: "version");

            migrationBuilder.AddColumn<decimal>(
                name: "base_price",
                table: "OrderItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "discount_amount",
                table: "OrderItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "final_price",
                table: "OrderItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ip_address",
                table: "Logs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_agent",
                table: "Logs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_type",
                table: "Logs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_default",
                table: "AdjustmentRules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_user_id",
                table: "Logs",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id");
        }
    }
}
