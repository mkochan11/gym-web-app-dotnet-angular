using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "GymMemberships");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "GymMemberships");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationRequestedDate",
                table: "GymMemberships",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveEndDate",
                table: "GymMemberships",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GymMemberships",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationRequestedDate",
                table: "GymMemberships");

            migrationBuilder.DropColumn(
                name: "EffectiveEndDate",
                table: "GymMemberships");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GymMemberships");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "GymMemberships",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "GymMemberships",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
