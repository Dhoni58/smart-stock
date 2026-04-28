using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierToMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "WarehouseMovements",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseMovements_SupplierId",
                table: "WarehouseMovements",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseMovements_Suppliers_SupplierId",
                table: "WarehouseMovements",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseMovements_Suppliers_SupplierId",
                table: "WarehouseMovements");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseMovements_SupplierId",
                table: "WarehouseMovements");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "WarehouseMovements");
        }
    }
}
