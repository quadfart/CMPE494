using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserSensorDataRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "tblSensorData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tblSensorData_UserId",
                table: "tblSensorData",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tblSensorData_tblUsers_UserId",
                table: "tblSensorData",
                column: "UserId",
                principalTable: "tblUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tblSensorData_tblUsers_UserId",
                table: "tblSensorData");

            migrationBuilder.DropIndex(
                name: "IX_tblSensorData_UserId",
                table: "tblSensorData");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "tblSensorData");
        }
    }
}
