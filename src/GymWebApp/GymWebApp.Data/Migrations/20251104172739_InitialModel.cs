using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaseUserEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: true),
                    Removed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseUserEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditableEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HourlyRate = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RepetitionsNumber = table.Column<int>(type: "int", nullable: true),
                    SeriesNumber = table.Column<int>(type: "int", nullable: true),
                    RestTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    TrainingPlanSessionId = table.Column<int>(type: "int", nullable: true),
                    MuscleGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrainerId = table.Column<int>(type: "int", nullable: true),
                    MaxParticipantNumber = table.Column<int>(type: "int", nullable: true),
                    TrainingTypeId = table.Column<int>(type: "int", nullable: true),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: true),
                    GroupTraining_Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: true),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GroupTrainingId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GroupTrainingParticipation_IsCancelled = table.Column<bool>(type: "bit", nullable: true),
                    GroupTrainingParticipation_CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MembershipPlanId = table.Column<int>(type: "int", nullable: true),
                    GymMembership_StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GymMembership_EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GymMembership_ClientId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    GymMembership_IsCancelled = table.Column<bool>(type: "bit", nullable: true),
                    GymMembership_CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IndividualTraining_TrainerId = table.Column<int>(type: "int", nullable: true),
                    IndividualTraining_ClientId = table.Column<int>(type: "int", nullable: true),
                    IndividualTraining_Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MembershipPlan_Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    DurationTime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DurationInMonths = table.Column<int>(type: "int", nullable: true),
                    CanReserveTrainings = table.Column<bool>(type: "bit", nullable: true),
                    CanAccessGroupTraining = table.Column<bool>(type: "bit", nullable: true),
                    CanAccessPersonalTraining = table.Column<bool>(type: "bit", nullable: true),
                    CanReceiveTrainingPlans = table.Column<bool>(type: "bit", nullable: true),
                    MaxTrainingsPerMonth = table.Column<int>(type: "int", nullable: true),
                    MembershipPlan_IsActive = table.Column<bool>(type: "bit", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GymMembershipId = table.Column<int>(type: "int", nullable: true),
                    PaymentMethod = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Shift_EmployeeId = table.Column<int>(type: "int", nullable: true),
                    TrainingPlan_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrainingPlan_Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrainingPlan_ClientId = table.Column<int>(type: "int", nullable: true),
                    TrainingPlan_TrainerId = table.Column<int>(type: "int", nullable: true),
                    TrainingPlan_StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrainingPlan_EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrainingPlan_IsActive = table.Column<bool>(type: "bit", nullable: true),
                    TrainingPlanId = table.Column<int>(type: "int", nullable: true),
                    SessionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrainingPlanSession_Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrainingType_Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrainingType_Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrainingType_DifficultyLevel = table.Column<int>(type: "int", nullable: true),
                    TrainingType_IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Removed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditableEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_AuditableEntity_GroupTrainingId",
                        column: x => x.GroupTrainingId,
                        principalTable: "AuditableEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_AuditableEntity_GymMembershipId",
                        column: x => x.GymMembershipId,
                        principalTable: "AuditableEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_AuditableEntity_MembershipPlanId",
                        column: x => x.MembershipPlanId,
                        principalTable: "AuditableEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_AuditableEntity_TrainingPlanId",
                        column: x => x.TrainingPlanId,
                        principalTable: "AuditableEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_AuditableEntity_TrainingPlanSessionId",
                        column: x => x.TrainingPlanSessionId,
                        principalTable: "AuditableEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_AuditableEntity_TrainingTypeId",
                        column: x => x.TrainingTypeId,
                        principalTable: "AuditableEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_BaseUserEntity_ClientId",
                        column: x => x.ClientId,
                        principalTable: "BaseUserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_BaseUserEntity_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "BaseUserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_BaseUserEntity_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "BaseUserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_BaseUserEntity_GymMembership_ClientId",
                        column: x => x.GymMembership_ClientId,
                        principalTable: "BaseUserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_BaseUserEntity_IndividualTraining_ClientId",
                        column: x => x.IndividualTraining_ClientId,
                        principalTable: "BaseUserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_BaseUserEntity_IndividualTraining_TrainerId",
                        column: x => x.IndividualTraining_TrainerId,
                        principalTable: "BaseUserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_BaseUserEntity_Shift_EmployeeId",
                        column: x => x.Shift_EmployeeId,
                        principalTable: "BaseUserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_BaseUserEntity_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "BaseUserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_BaseUserEntity_TrainingPlan_ClientId",
                        column: x => x.TrainingPlan_ClientId,
                        principalTable: "BaseUserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_BaseUserEntity_TrainingPlan_TrainerId",
                        column: x => x.TrainingPlan_TrainerId,
                        principalTable: "BaseUserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditableEntity_BaseUserEntity_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "BaseUserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_ClientId",
                table: "AuditableEntity",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_CreatedById",
                table: "AuditableEntity",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_EmployeeId",
                table: "AuditableEntity",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_GroupTrainingId",
                table: "AuditableEntity",
                column: "GroupTrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_GymMembership_ClientId",
                table: "AuditableEntity",
                column: "GymMembership_ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_GymMembershipId",
                table: "AuditableEntity",
                column: "GymMembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_IndividualTraining_ClientId",
                table: "AuditableEntity",
                column: "IndividualTraining_ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_IndividualTraining_TrainerId",
                table: "AuditableEntity",
                column: "IndividualTraining_TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_MembershipPlanId",
                table: "AuditableEntity",
                column: "MembershipPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_Shift_EmployeeId",
                table: "AuditableEntity",
                column: "Shift_EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_TrainerId",
                table: "AuditableEntity",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_TrainingPlan_ClientId",
                table: "AuditableEntity",
                column: "TrainingPlan_ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_TrainingPlan_TrainerId",
                table: "AuditableEntity",
                column: "TrainingPlan_TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_TrainingPlanId",
                table: "AuditableEntity",
                column: "TrainingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_TrainingPlanSessionId",
                table: "AuditableEntity",
                column: "TrainingPlanSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_TrainingTypeId",
                table: "AuditableEntity",
                column: "TrainingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditableEntity_UpdatedById",
                table: "AuditableEntity",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditableEntity");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "BaseUserEntity");
        }
    }
}
