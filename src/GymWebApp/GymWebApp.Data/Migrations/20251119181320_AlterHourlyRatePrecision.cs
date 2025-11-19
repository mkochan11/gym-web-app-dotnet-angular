using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterHourlyRatePrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "HourlyRate",
                table: "AuditableEntity",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "HourlyRate",
                table: "AuditableEntity",
                type: "decimal(3,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);
        }
    }
}
