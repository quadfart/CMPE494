using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SensorDataUpdateForRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblSensorData_tblPlants_PlantId",
                table: "tblSensorData");

            migrationBuilder.DropForeignKey(
                name: "FK_tblSensorData_tblUsers_UserId",
                table: "tblSensorData");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSensorData_tblPlants_PlantId",
                table: "tblSensorData",
                column: "PlantId",
                principalTable: "tblPlants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSensorData_tblUsers_UserId",
                table: "tblSensorData",
                column: "UserId",
                principalTable: "tblUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblSensorData_tblPlants_PlantId",
                table: "tblSensorData");

            migrationBuilder.DropForeignKey(
                name: "FK_tblSensorData_tblUsers_UserId",
                table: "tblSensorData");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSensorData_tblPlants_PlantId",
                table: "tblSensorData",
                column: "PlantId",
                principalTable: "tblPlants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tblSensorData_tblUsers_UserId",
                table: "tblSensorData",
                column: "UserId",
                principalTable: "tblUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
