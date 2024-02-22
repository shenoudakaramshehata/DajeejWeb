using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dajeej.Migrations.Dajeej
{
    public partial class notificationsModelMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityTypeNotifies",
                columns: table => new
                {
                    EntityTypeNotifyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypeNotifies", x => x.EntityTypeNotifyId);
                });

            migrationBuilder.CreateTable(
                name: "PublicDevices",
                columns: table => new
                {
                    PublicDeviceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAndroiodDevice = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicDevices", x => x.PublicDeviceId);
                    table.ForeignKey(
                        name: "FK_PublicDevices_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublicNotifications",
                columns: table => new
                {
                    PublicNotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntityTypeNotifyId = table.Column<int>(type: "int", nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicNotifications", x => x.PublicNotificationId);
                    table.ForeignKey(
                        name: "FK_PublicNotifications_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublicNotifications_EntityTypeNotifies_EntityTypeNotifyId",
                        column: x => x.EntityTypeNotifyId,
                        principalTable: "EntityTypeNotifies",
                        principalColumn: "EntityTypeNotifyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PublicNotificationDevices",
                columns: table => new
                {
                    PublicNotificationDeviceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicNotificationId = table.Column<int>(type: "int", nullable: false),
                    PublicDeviceId = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicNotificationDevices", x => x.PublicNotificationDeviceId);
                    table.ForeignKey(
                        name: "FK_PublicNotificationDevices_PublicDevices_PublicDeviceId",
                        column: x => x.PublicDeviceId,
                        principalTable: "PublicDevices",
                        principalColumn: "PublicDeviceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublicNotificationDevices_PublicNotifications_PublicNotificationId",
                        column: x => x.PublicNotificationId,
                        principalTable: "PublicNotifications",
                        principalColumn: "PublicNotificationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EntityTypeNotifies",
                columns: new[] { "EntityTypeNotifyId", "TitleAr", "TitleEn" },
                values: new object[] { 1, "عناصر", "Items" });

            migrationBuilder.InsertData(
                table: "EntityTypeNotifies",
                columns: new[] { "EntityTypeNotifyId", "TitleAr", "TitleEn" },
                values: new object[] { 2, "متاجر", "Shops" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicDevices_CountryId",
                table: "PublicDevices",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicNotificationDevices_PublicDeviceId",
                table: "PublicNotificationDevices",
                column: "PublicDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicNotificationDevices_PublicNotificationId",
                table: "PublicNotificationDevices",
                column: "PublicNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicNotifications_CountryId",
                table: "PublicNotifications",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicNotifications_EntityTypeNotifyId",
                table: "PublicNotifications",
                column: "EntityTypeNotifyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicNotificationDevices");

            migrationBuilder.DropTable(
                name: "PublicDevices");

            migrationBuilder.DropTable(
                name: "PublicNotifications");

            migrationBuilder.DropTable(
                name: "EntityTypeNotifies");
        }
    }
}
