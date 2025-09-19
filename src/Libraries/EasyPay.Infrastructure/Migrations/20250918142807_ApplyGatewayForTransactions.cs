using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplyGatewayForTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GatewayName",
                table: "Transaction",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewayToken",
                table: "Transaction",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_GatewayToken",
                table: "Transaction",
                column: "GatewayToken",
                unique: true,
                filter: "[GatewayToken] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_GatewayToken",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "GatewayName",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "GatewayToken",
                table: "Transaction");
        }
    }
}
