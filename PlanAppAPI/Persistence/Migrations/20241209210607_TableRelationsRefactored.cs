using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TableRelationsRefactored : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblDiseases_tblPlants_PlantId",
                table: "tblDiseases");

            migrationBuilder.DropIndex(
                name: "IX_tblDiseases_PlantId",
                table: "tblDiseases");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_tblDiseases_PlantId",
                table: "tblDiseases",
                column: "PlantId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblDiseases_tblPlants_PlantId",
                table: "tblDiseases",
                column: "PlantId",
                principalTable: "tblPlants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
