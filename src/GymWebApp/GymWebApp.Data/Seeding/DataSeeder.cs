using GymWebApp.Data.Entities;
using GymWebApp.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GymWebApp.Data.Seeding;

public class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        try
        {
            if (await context.MembershipPlans.AnyAsync())
            {
                Log.Information("Database already contains data. Skipping seeding.");
                return;
            }

            Log.Information("Starting database seeding...");

            var managerUser = await userManager.FindByEmailAsync("manager@gymWebApp.com");
            var adminUser = await userManager.FindByEmailAsync("admin@gymWebApp.com");
            var trainer1User = await userManager.FindByEmailAsync("trainer1@gymWebApp.com");
            var trainer2User = await userManager.FindByEmailAsync("trainer2@gymWebApp.com");

            if (managerUser == null || adminUser == null || trainer1User == null || trainer2User == null)
            {
                throw new InvalidOperationException("Required users not found in database.");
            }

            Log.Information("Using manager ID: {ManagerId}", managerUser.Id);
            Log.Information("Using admin ID: {AdminId}", adminUser.Id);
            Log.Information("Using trainer1 ID: {Trainer1Id}, trainer2 ID: {Trainer2Id}", trainer1User.Id, trainer2User.Id);

            var trainingTypes = await CreateTrainingTypesAsync(context, managerUser.Id);
            var membershipPlans = await CreateMembershipPlansAsync(context, managerUser.Id);
            await CreateGymMembershipsAsync(context, membershipPlans, adminUser.Id);
            await CreateEmploymentsAsync(context, managerUser.Id);
            await CreateShiftsAsync(context, managerUser.Id);
            var groupTrainings = await CreateGroupTrainingsAsync(context, trainingTypes, trainer1User.Id);
            await CreateIndividualTrainingsAsync(context, trainer2User.Id);
            await CreateTrainingPlansWithSessionsAndExercisesAsync(context, trainer1User.Id);
            await CreatePaymentsAsync(context, adminUser.Id);

            await context.SaveChangesAsync();
            Log.Information("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private static async Task<List<TrainingType>> CreateTrainingTypesAsync(ApplicationDbContext context, string managerId)
    {
        var trainingTypes = new List<TrainingType>
            {
                new() {
                    Name = "Yoga",
                    Description = "Gentle yoga session focusing on flexibility and relaxation",
                    DifficultyLevel = 1,
                    IsActive = true,
                    CreatedById = managerId
                },
                new() {
                    Name = "HIIT",
                    Description = "High Intensity Interval Training for maximum calorie burn",
                    DifficultyLevel = 5,
                    IsActive = true,
                    CreatedById = managerId
                },
                new() {
                    Name = "Strength Training",
                    Description = "Weight training focusing on building muscle and strength",
                    DifficultyLevel = 3,
                    IsActive = true,
                    CreatedById = managerId
                },
                new() {
                    Name = "Pilates",
                    Description = "Core strengthening and body conditioning exercises",
                    DifficultyLevel = 2,
                    IsActive = true,
                    CreatedById = managerId
                }
            };

        context.TrainingTypes.AddRange(trainingTypes);
        await context.SaveChangesAsync();
        return trainingTypes;
    }

    private static async Task<List<MembershipPlan>> CreateMembershipPlansAsync(ApplicationDbContext context, string managerId)
    {
        var membershipPlans = new List<MembershipPlan>
            {
                new() {
                    Type = "Basic",
                    Description = "Basic access to the gym.",
                    Price = 49.99m,
                    DurationTime = "3 Months",
                    DurationInMonths = 3,
                    CanReserveTrainings = false,
                    CanAccessGroupTraining = false,
                    CanAccessPersonalTraining = false,
                    CanReceiveTrainingPlans = false,
                    MaxTrainingsPerMonth = null,
                    IsActive = true,
                    CreatedById = managerId
                },
                new() {
                    Type = "Premium",
                    Description = "Full access to all gym facilities including group trainings",
                    Price = 89.99m,
                    DurationTime = "3 Months",
                    DurationInMonths = 3,
                    CanReserveTrainings = true,
                    CanAccessGroupTraining = true,
                    CanAccessPersonalTraining = false,
                    CanReceiveTrainingPlans = true,
                    MaxTrainingsPerMonth = 8,
                    IsActive = true,
                    CreatedById = managerId
                },
                new() {
                    Type = "VIP",
                    Description = "All-inclusive membership with personal training sessions",
                    Price = 149.99m,
                    DurationTime = "3 Months",
                    DurationInMonths = 3,
                    CanReserveTrainings = true,
                    CanAccessGroupTraining = true,
                    CanAccessPersonalTraining = true,
                    CanReceiveTrainingPlans = true,
                    MaxTrainingsPerMonth = 12,
                    IsActive = true,
                    CreatedById = managerId
                }
            };

        context.MembershipPlans.AddRange(membershipPlans);
        await context.SaveChangesAsync();
        return membershipPlans;
    }

    private static async Task CreateGymMembershipsAsync(ApplicationDbContext context, List<MembershipPlan> membershipPlans, string adminId)
    {
        var clients = await context.Clients.ToListAsync();
        var gymMemberships = new List<GymMembership>();
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddMonths(3);

        for (int i = 0; i < clients.Count; i++)
        {
            var plan = membershipPlans[i % membershipPlans.Count];
            gymMemberships.Add(new GymMembership
            {
                MembershipPlanId = plan.Id,
                ClientId = clients[i].Id,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true,
                CreatedById = adminId
            });
        }

        context.GymMemberships.AddRange(gymMemberships);
        await context.SaveChangesAsync();
    }

    private static async Task CreateEmploymentsAsync(ApplicationDbContext context, string managerId)
    {
        var employees = await context.Employees.ToListAsync();
        employees.RemoveAll(e => e.Role == EmployeeRole.Owner);
        var employments = new List<Employment>();

        foreach (var employee in employees)
        {
            employments.Add(new Employment
            {
                EmployeeId = employee.Id,
                StartDate = DateTime.UtcNow,
                HourlyRate = employee.Role switch
                {
                    EmployeeRole.Manager => 35.00m,
                    EmployeeRole.Trainer => 25.00m,
                    EmployeeRole.Receptionist => 18.00m,
                    _ => 20.00m
                },
                CreatedById = managerId
            });
        }

        context.Employments.AddRange(employments);
        await context.SaveChangesAsync();
    }

    private static async Task CreateShiftsAsync(ApplicationDbContext context, string managerId)
    {
        var employees = await context.Employees.ToListAsync();
        var shifts = new List<Shift>();
        var baseDate = DateTime.UtcNow.Date;

        foreach (var employee in employees)
        {
            for (int i = 0; i < 7; i++)
            {
                var shiftDate = baseDate.AddDays(i);
                shifts.Add(new Shift
                {
                    EmployeeId = employee.Id,
                    StartTime = shiftDate.AddHours(8),
                    EndTime = shiftDate.AddHours(12),
                    CreatedById = managerId
                });

                if (employee.Id % 2 == 0)
                {
                    shifts.Add(new Shift
                    {
                        EmployeeId = employee.Id,
                        StartTime = shiftDate.AddHours(14),
                        EndTime = shiftDate.AddHours(18),
                        CreatedById = managerId
                    });
                }
            }
        }

        context.Shifts.AddRange(shifts);
        await context.SaveChangesAsync();
    }

    private static async Task<List<GroupTraining>> CreateGroupTrainingsAsync(ApplicationDbContext context, List<TrainingType> trainingTypes, string trainerId)
    {
        var trainers = await context.Employees.Where(e => e.Role == EmployeeRole.Trainer).ToListAsync();
        var groupTrainings = new List<GroupTraining>();
        var baseDate = DateTime.UtcNow.AddDays(1).Date;

        for (int i = 0; i < 10; i++)
        {
            var trainingDate = baseDate.AddDays(i);
            var trainingType = trainingTypes[i % trainingTypes.Count];
            var trainer = trainers[i % trainers.Count];

            groupTrainings.Add(new GroupTraining
            {
                TrainerId = trainer.Id,
                TrainingTypeId = trainingType.Id,
                Date = trainingDate.AddHours(10 + (i % 6)),
                Duration = TimeSpan.FromHours(1),
                Description = $"{trainingType.Name} session with {trainer.Name}",
                MaxParticipantNumber = 15,
                DifficultyLevel = trainingType.DifficultyLevel ?? 3,
                CreatedById = trainerId
            });
        }

        context.GroupTrainings.AddRange(groupTrainings);
        await context.SaveChangesAsync();
        await CreateGroupTrainingParticipationsAsync(context, groupTrainings, trainerId);
        return groupTrainings;
    }

    private static async Task CreateGroupTrainingParticipationsAsync(ApplicationDbContext context, List<GroupTraining> groupTrainings, string adminId)
    {
        var clients = await context.Clients.ToListAsync();
        var participations = new List<GroupTrainingParticipation>();

        foreach (var training in groupTrainings.Take(5))
        {
            var participants = clients.Take(Random.Shared.Next(3, Math.Min(9, clients.Count + 1))).ToList();
            foreach (var client in participants)
            {
                participations.Add(new GroupTrainingParticipation
                {
                    GroupTrainingId = training.Id,
                    ClientId = client.Id,
                    RegistrationDate = DateTime.UtcNow.AddDays(-1),
                    CreatedById = adminId
                });
            }
        }

        context.GroupTrainingParticipations.AddRange(participations);
        await context.SaveChangesAsync();
    }

    private static async Task CreateIndividualTrainingsAsync(ApplicationDbContext context, string trainerId)
    {
        var trainers = await context.Employees.Where(e => e.Role == EmployeeRole.Trainer).ToListAsync();
        var clients = await context.Clients.ToListAsync();
        var individualTrainings = new List<IndividualTraining>();
        var baseDate = DateTime.UtcNow.AddDays(1).Date;

        for (int i = 0; i < 8; i++)
        {
            var trainingDate = baseDate.AddDays(i);
            var trainer = trainers[i % trainers.Count];
            var client = clients[i % clients.Count];

            individualTrainings.Add(new IndividualTraining
            {
                TrainerId = trainer.Id,
                ClientId = client.Id,
                Date = trainingDate.AddHours(14 + (i % 4)),
                Duration = TimeSpan.FromMinutes(60),
                Description = $"Personal training session with {trainer.Name}",
                CreatedById = trainerId
            });
        }

        context.IndividualTrainings.AddRange(individualTrainings);
        await context.SaveChangesAsync();
    }

    private static async Task CreateTrainingPlansWithSessionsAndExercisesAsync(ApplicationDbContext context, string trainerId)
    {
        var trainers = await context.Employees.Where(e => e.Role == EmployeeRole.Trainer).ToListAsync();
        var clients = await context.Clients.ToListAsync();
        var trainingPlans = new List<TrainingPlan>();

        for (int i = 0; i < 3; i++)
        {
            var trainer = trainers[i % trainers.Count];
            var client = clients[i % clients.Count];

            trainingPlans.Add(new TrainingPlan
            {
                Name = $"Training Plan {i + 1} - {client.Name}",
                Description = $"Customized training plan for {client.Name}",
                ClientId = client.Id,
                TrainerId = trainer.Id,
                StartDate = DateTime.UtcNow.AddDays(-7),
                EndDate = DateTime.UtcNow.AddDays(21),
                IsActive = true,
                CreatedById = trainerId
            });
        }

        context.TrainingPlans.AddRange(trainingPlans);
        await context.SaveChangesAsync();

        var sessions = new List<TrainingPlanSession>();
        foreach (var plan in trainingPlans)
        {
            for (int j = 0; j < 3; j++)
            {
                sessions.Add(new TrainingPlanSession
                {
                    TrainingPlanId = plan.Id,
                    SessionDate = DateTime.UtcNow.AddDays(j * 7),
                    Notes = $"Session {j + 1} notes",
                    CreatedById = trainerId
                });
            }
        }

        context.TrainingPlanSessions.AddRange(sessions);
        await context.SaveChangesAsync();

        var exercises = new List<Exercise>();
        var muscleGroups = new[] { "Chest", "Back", "Legs", "Shoulders", "Arms", "Core" };

        foreach (var session in sessions)
        {
            for (int k = 0; k < 4; k++)
            {
                exercises.Add(new Exercise
                {
                    Name = $"{muscleGroups[k % muscleGroups.Length]} Exercise {k + 1}",
                    Description = $"Description for {muscleGroups[k % muscleGroups.Length]} exercise",
                    RepetitionsNumber = 10 + (k * 2),
                    SeriesNumber = 3 + (k % 2),
                    RestTime = TimeSpan.FromSeconds(60 + (k * 15)),
                    TrainingPlanSessionId = session.Id,
                    MuscleGroup = muscleGroups[k % muscleGroups.Length],
                    Order = k + 1,
                    Instructions = $"Proper form instructions for {muscleGroups[k % muscleGroups.Length]} exercise",
                    CreatedById = trainerId
                });
            }
        }

        context.Exercises.AddRange(exercises);
        await context.SaveChangesAsync();
    }

    private static async Task CreatePaymentsAsync(ApplicationDbContext context, string adminId)
    {
        var gymMemberships = await context.GymMemberships.Include(gm => gm.MembershipPlan).ToListAsync();
        var payments = new List<Payment>();
        var paymentDate = DateTime.UtcNow.AddDays(-5);

        foreach (var membership in gymMemberships)
        {
            payments.Add(new Payment
            {
                GymMembershipId = membership.Id,
                PaymentDate = paymentDate,
                PaymentMethod = PaymentMethod.Card,
                Amount = membership.MembershipPlan.Price,
                TransactionId = $"TXN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                ReferenceNumber = $"REF{membership.Id:0000}",
                IsSuccessful = true,
                ProcessedAt = paymentDate.AddMinutes(5),
                ProcessedBy = "System",
                CreatedById = adminId
            });
        }

        context.Payments.AddRange(payments);
        await context.SaveChangesAsync();
    }
}