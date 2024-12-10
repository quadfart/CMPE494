using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PlantSensorDataRelationUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tblSensorData_PlantId",
                table: "tblSensorData");

            migrationBuilder.CreateIndex(
                name: "IX_tblSensorData_PlantId",
                table: "tblSensorData",
                column: "PlantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tblSensorData_PlantId",
                table: "tblSensorData");

            migrationBuilder.CreateIndex(
                name: "IX_tblSensorData_PlantId",
                table: "tblSensorData",
                column: "PlantId",
                unique: true);
        }
    }
}
