using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SensorSerialNumberAddedEndpointRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoilMoisture",
                table: "tblSensorLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SensorSerialNumber",
                table: "tblSensorData",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoilMoisture",
                table: "tblSensorLogs");

            migrationBuilder.DropColumn(
                name: "SensorSerialNumber",
                table: "tblSensorData");
        }
    }
}
