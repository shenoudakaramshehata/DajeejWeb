using Microsoft.EntityFrameworkCore.Migrations;

namespace Dajeej.Migrations.Dajeej
{
    public partial class UpdateShopMMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionTLAR",
                table: "Shops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionTLEN",
                table: "Shops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instgram",
                table: "Shops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ShopImages",
                columns: table => new
                {
                    ShopImagesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShopId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopImages", x => x.ShopImagesId);
                    table.ForeignKey(
                        name: "FK_ShopImages_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "ShopId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShopImages_ShopId",
                table: "ShopImages",
                column: "ShopId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopImages");

            migrationBuilder.DropColumn(
                name: "DescriptionTLAR",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "DescriptionTLEN",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "Instgram",
                table: "Shops");
        }
    }
}
