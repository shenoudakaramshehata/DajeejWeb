using Microsoft.EntityFrameworkCore.Migrations;

namespace Dajeej.Migrations.Dajeej
{
    public partial class bannerModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    BannerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    EntityTypeNotifyId = table.Column<int>(type: "int", nullable: false),
                    EntityID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.BannerId);
                    table.ForeignKey(
                        name: "FK_Banners_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Banners_EntityTypeNotifies_EntityTypeNotifyId",
                        column: x => x.EntityTypeNotifyId,
                        principalTable: "EntityTypeNotifies",
                        principalColumn: "EntityTypeNotifyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banners_CountryId",
                table: "Banners",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_EntityTypeNotifyId",
                table: "Banners",
                column: "EntityTypeNotifyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");
        }
    }
}
