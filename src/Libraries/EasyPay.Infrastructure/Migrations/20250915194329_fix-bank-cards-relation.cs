using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixbankcardsrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerUserId",
                table: "BankCards",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BankCards_OwnerUserId",
                table: "BankCards",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankCards_NaturalUser_OwnerUserId",
                table: "BankCards",
                column: "OwnerUserId",
                principalTable: "NaturalUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankCards_NaturalUser_OwnerUserId",
                table: "BankCards");

            migrationBuilder.DropIndex(
                name: "IX_BankCards_OwnerUserId",
                table: "BankCards");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "BankCards");
        }
    }
}
