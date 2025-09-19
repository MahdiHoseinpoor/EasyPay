using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyPay.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changebills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "BillBase",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "BillBase",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Unpaid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "BillBase");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BillBase");
        }
    }
}
