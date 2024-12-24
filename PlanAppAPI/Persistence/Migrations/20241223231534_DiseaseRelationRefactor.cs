using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DiseaseRelationRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblPlants_tblDiseases_DiseaseId",
                table: "tblPlants");

            migrationBuilder.DropIndex(
                name: "IX_tblPlants_DiseaseId",
                table: "tblPlants");

            migrationBuilder.DropColumn(
                name: "DiseaseId",
                table: "tblPlants");

            migrationBuilder.AddColumn<int>(
                name: "DiseaseId",
                table: "tblSensorData",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblSensorData_DiseaseId",
                table: "tblSensorData",
                column: "DiseaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSensorData_tblDiseases_DiseaseId",
                table: "tblSensorData",
                column: "DiseaseId",
                principalTable: "tblDiseases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblSensorData_tblDiseases_DiseaseId",
                table: "tblSensorData");

            migrationBuilder.DropIndex(
                name: "IX_tblSensorData_DiseaseId",
                table: "tblSensorData");

            migrationBuilder.DropColumn(
                name: "DiseaseId",
                table: "tblSensorData");

            migrationBuilder.AddColumn<int>(
                name: "DiseaseId",
                table: "tblPlants",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_tblPlants_DiseaseId",
                table: "tblPlants",
                column: "DiseaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblPlants_tblDiseases_DiseaseId",
                table: "tblPlants",
                column: "DiseaseId",
                principalTable: "tblDiseases",
                principalColumn: "Id");
        }
    }
}
