using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class Finalcheckdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_user_role_UserRoleId",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "create_at",
                table: "complaint_image",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "update_at",
                table: "cart",
                newName: "updated_at");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "wallet",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "locked_balance",
                table: "wallet",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "wallet",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "user_security",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "user_role_name",
                table: "user_role",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "user_role_description",
                table: "user_role",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UserRoleId",
                table: "user",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "product_review_image",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "adjusted_amount",
                table: "order_item",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_read",
                table: "notification",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "read_at",
                table: "notification",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "conversation_id",
                table: "message",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "category",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_checked_out",
                table: "cart",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_user_user_role_UserRoleId",
                table: "user",
                column: "UserRoleId",
                principalTable: "user_role",
                principalColumn: "user_role_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_user_role_UserRoleId",
                table: "user");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "wallet");

            migrationBuilder.DropColumn(
                name: "locked_balance",
                table: "wallet");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "wallet");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "user_security");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "product_review_image");

            migrationBuilder.DropColumn(
                name: "adjusted_amount",
                table: "order_item");

            migrationBuilder.DropColumn(
                name: "is_read",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "read_at",
                table: "notification");

            migrationBuilder.DropColumn(
                name: "conversation_id",
                table: "message");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "category");

            migrationBuilder.DropColumn(
                name: "is_checked_out",
                table: "cart");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "complaint_image",
                newName: "create_at");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "cart",
                newName: "update_at");

            migrationBuilder.AlterColumn<string>(
                name: "user_role_name",
                table: "user_role",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "user_role_description",
                table: "user_role",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "UserRoleId",
                table: "user",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_user_user_role_UserRoleId",
                table: "user",
                column: "UserRoleId",
                principalTable: "user_role",
                principalColumn: "user_role_id");
        }
    }
}
