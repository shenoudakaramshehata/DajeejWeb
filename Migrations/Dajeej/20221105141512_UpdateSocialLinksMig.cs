using Microsoft.EntityFrameworkCore.Migrations;

namespace Dajeej.Migrations.Dajeej
{
    public partial class UpdateSocialLinksMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YoutubeLink",
                table: "SoicialMidiaLinks",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YoutubeLink",
                table: "SoicialMidiaLinks");
        }
    }
}
