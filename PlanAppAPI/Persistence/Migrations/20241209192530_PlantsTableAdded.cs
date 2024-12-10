using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PlantsTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblPlants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModTemp = table.Column<int>(type: "integer", nullable: false),
                    SoilType = table.Column<string>(type: "text", nullable: false),
                    LightNeed = table.Column<int>(type: "integer", nullable: false),
                    HumidityLevel = table.Column<int>(type: "integer", nullable: false),
                    WateringFrequency = table.Column<string>(type: "text", nullable: false),
                    IrrigationAmount = table.Column<int>(type: "integer", nullable: false),
                    ScientificName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblPlants", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblPlants");
        }
    }
}
