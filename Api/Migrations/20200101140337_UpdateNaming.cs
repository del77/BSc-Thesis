using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class UpdateNaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Properties_HeightAboveSeaLevel",
                table: "Routes");

            migrationBuilder.AddColumn<int>(
                name: "Properties_TerrainLevel",
                table: "Routes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Properties_TerrainLevel",
                table: "Routes");

            migrationBuilder.AddColumn<int>(
                name: "Properties_HeightAboveSeaLevel",
                table: "Routes",
                type: "int",
                nullable: true);
        }
    }
}
