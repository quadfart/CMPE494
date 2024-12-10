using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SensorDataLogTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Moisture",
                table: "tblSensorData");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "tblSensorData");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "tblSensorData");

            migrationBuilder.CreateTable(
                name: "tblSensorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Temperature = table.Column<int>(type: "integer", nullable: false),
                    Moisture = table.Column<int>(type: "integer", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SensorDataId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblSensorLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tblSensorLogs_tblSensorData_SensorDataId",
                        column: x => x.SensorDataId,
                        principalTable: "tblSensorData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblSensorLogs_SensorDataId",
                table: "tblSensorLogs",
                column: "SensorDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblSensorLogs");

            migrationBuilder.AddColumn<int>(
                name: "Moisture",
                table: "tblSensorData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Temperature",
                table: "tblSensorData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "tblSensorData",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
