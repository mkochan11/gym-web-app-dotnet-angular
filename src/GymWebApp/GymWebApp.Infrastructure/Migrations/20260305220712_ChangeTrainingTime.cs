using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTrainingTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "IndividualTrainings");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "GroupTrainings");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "IndividualTrainings",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "GroupTrainings",
                newName: "StartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "IndividualTrainings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "GroupTrainings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "IndividualTrainings");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "GroupTrainings");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "IndividualTrainings",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "GroupTrainings",
                newName: "Date");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "IndividualTrainings",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "GroupTrainings",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
