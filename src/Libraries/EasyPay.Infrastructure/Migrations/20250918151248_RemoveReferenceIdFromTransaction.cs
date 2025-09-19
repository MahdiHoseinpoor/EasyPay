using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReferenceIdFromTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_ReferenceId",
                table: "Transaction");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ReferenceId",
                table: "Transaction",
                column: "ReferenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transaction_ReferenceId",
                table: "Transaction");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_ReferenceId",
                table: "Transaction",
                column: "ReferenceId",
                unique: true);
        }
    }
}
