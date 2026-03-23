using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentDate",
                table: "Payments",
                newName: "DueDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidDate",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidDate",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Payments",
                newName: "PaymentDate");
        }
    }
}
