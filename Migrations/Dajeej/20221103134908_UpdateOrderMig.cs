using Microsoft.EntityFrameworkCore.Migrations;

namespace Dajeej.Migrations.Dajeej
{
    public partial class UpdateOrderMig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentMehods_PaymentMehodPaymentMethodId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentMehodPaymentMethodId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMehodPaymentMethodId",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMethodId",
                table: "Orders",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentMehods_PaymentMethodId",
                table: "Orders",
                column: "PaymentMethodId",
                principalTable: "PaymentMehods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PaymentMehods_PaymentMethodId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PaymentMethodId",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMehodPaymentMethodId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentMehodPaymentMethodId",
                table: "Orders",
                column: "PaymentMehodPaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PaymentMehods_PaymentMehodPaymentMethodId",
                table: "Orders",
                column: "PaymentMehodPaymentMethodId",
                principalTable: "PaymentMehods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
