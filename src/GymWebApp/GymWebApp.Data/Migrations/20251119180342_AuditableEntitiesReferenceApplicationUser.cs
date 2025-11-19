using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AuditableEntitiesReferenceApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_ClientId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_CreatedById",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_EmployeeId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_GymMembership_ClientId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_IndividualTraining_ClientId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_IndividualTraining_TrainerId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_Shift_EmployeeId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_TrainerId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_TrainingPlan_ClientId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_TrainingPlan_TrainerId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_UpdatedById",
                table: "AuditableEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BaseUserEntity",
                table: "BaseUserEntity");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "BaseUserEntity");

            migrationBuilder.RenameTable(
                name: "BaseUserEntity",
                newName: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedById",
                table: "AuditableEntity",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "AuditableEntity",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Removed = table.Column<bool>(type: "bit", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<int>(type: "int", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_AspNetUsers_CreatedById",
                table: "AuditableEntity",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_AspNetUsers_UpdatedById",
                table: "AuditableEntity",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_Clients_ClientId",
                table: "AuditableEntity",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_Clients_GymMembership_ClientId",
                table: "AuditableEntity",
                column: "GymMembership_ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_Clients_IndividualTraining_ClientId",
                table: "AuditableEntity",
                column: "IndividualTraining_ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_Clients_TrainingPlan_ClientId",
                table: "AuditableEntity",
                column: "TrainingPlan_ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_Employees_EmployeeId",
                table: "AuditableEntity",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_Employees_IndividualTraining_TrainerId",
                table: "AuditableEntity",
                column: "IndividualTraining_TrainerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_Employees_Shift_EmployeeId",
                table: "AuditableEntity",
                column: "Shift_EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_Employees_TrainerId",
                table: "AuditableEntity",
                column: "TrainerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_Employees_TrainingPlan_TrainerId",
                table: "AuditableEntity",
                column: "TrainingPlan_TrainerId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_AspNetUsers_CreatedById",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_AspNetUsers_UpdatedById",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_Clients_ClientId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_Clients_GymMembership_ClientId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_Clients_IndividualTraining_ClientId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_Clients_TrainingPlan_ClientId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_Employees_EmployeeId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_Employees_IndividualTraining_TrainerId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_Employees_Shift_EmployeeId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_Employees_TrainerId",
                table: "AuditableEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditableEntity_Employees_TrainingPlan_TrainerId",
                table: "AuditableEntity");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "BaseUserEntity");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedById",
                table: "AuditableEntity",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedById",
                table: "AuditableEntity",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "BaseUserEntity",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "BaseUserEntity",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BaseUserEntity",
                table: "BaseUserEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_ClientId",
                table: "AuditableEntity",
                column: "ClientId",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_CreatedById",
                table: "AuditableEntity",
                column: "CreatedById",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_EmployeeId",
                table: "AuditableEntity",
                column: "EmployeeId",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_GymMembership_ClientId",
                table: "AuditableEntity",
                column: "GymMembership_ClientId",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_IndividualTraining_ClientId",
                table: "AuditableEntity",
                column: "IndividualTraining_ClientId",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_IndividualTraining_TrainerId",
                table: "AuditableEntity",
                column: "IndividualTraining_TrainerId",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_Shift_EmployeeId",
                table: "AuditableEntity",
                column: "Shift_EmployeeId",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_TrainerId",
                table: "AuditableEntity",
                column: "TrainerId",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_TrainingPlan_ClientId",
                table: "AuditableEntity",
                column: "TrainingPlan_ClientId",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_TrainingPlan_TrainerId",
                table: "AuditableEntity",
                column: "TrainingPlan_TrainerId",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditableEntity_BaseUserEntity_UpdatedById",
                table: "AuditableEntity",
                column: "UpdatedById",
                principalTable: "BaseUserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
