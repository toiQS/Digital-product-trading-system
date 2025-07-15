using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DPTS.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updatedbaddescprocess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EscrowProcesses",
                columns: table => new
                {
                    process_id = table.Column<string>(type: "text", nullable: false),
                    process_name = table.Column<string>(type: "text", nullable: false),
                    process_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    escrow_id = table.Column<string>(type: "text", nullable: false),
                    escrow_process_information = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscrowProcesses", x => x.process_id);
                    table.ForeignKey(
                        name: "FK_EscrowProcesses_Escrows_escrow_id",
                        column: x => x.escrow_id,
                        principalTable: "Escrows",
                        principalColumn: "escrow_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EscrowProcesses_escrow_id",
                table: "EscrowProcesses",
                column: "escrow_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EscrowProcesses");
        }
    }
}
