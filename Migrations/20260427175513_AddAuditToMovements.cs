using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditToMovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "WarehouseMovements",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseMovements_CreatedByUserId",
                table: "WarehouseMovements",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseMovements_Users_CreatedByUserId",
                table: "WarehouseMovements",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseMovements_Users_CreatedByUserId",
                table: "WarehouseMovements");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseMovements_CreatedByUserId",
                table: "WarehouseMovements");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "WarehouseMovements");
        }
    }
}
