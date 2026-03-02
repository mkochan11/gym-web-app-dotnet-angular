using GymWebApp.Application.Interfaces.Seeding;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GymWebApp.Infrastructure.Seeding;

public class DomainDataSeeder : IDomainDataSeeder
{
    private readonly ApplicationDbContext _context;

    public DomainDataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (await _context.MembershipPlans.AnyAsync())
        {
            Log.Information("Domain data already seeded. Skipping.");
            return;
        }

        Log.Information("Seeding domain data...");

        var admin = await GetUserIdByRole(UserRole.Admin);
        var manager = await GetUserIdByRole(UserRole.Manager);
        var trainer1 = await GetUserIdByRole(UserRole.Trainer, 0);
        var trainer2 = await GetUserIdByRole(UserRole.Trainer, 1);

        var trainingTypes = await CreateTrainingTypesAsync(manager);
        var membershipPlans = await CreateMembershipPlansAsync(manager);

        await CreateGymMembershipsAsync(membershipPlans, admin);
        await CreateEmploymentsAsync(manager);
        await CreateShiftsAsync(manager);

        var groupTrainings = await CreateGroupTrainingsAsync(trainingTypes, trainer1);
        await CreateIndividualTrainingsAsync(trainer2);

        await CreateTrainingPlansWithSessionsAndExercisesAsync(trainer1);
        await CreatePaymentsAsync(admin);

        Log.Information("Domain seeding completed successfully.");
    }

    private async Task<List<TrainingType>> CreateTrainingTypesAsync(string managerId)
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

        _context.TrainingTypes.AddRange(trainingTypes);
        await _context.SaveChangesAsync();
        return trainingTypes;
    }

    private async Task<List<MembershipPlan>> CreateMembershipPlansAsync(string managerId)
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

        _context.MembershipPlans.AddRange(membershipPlans);
        await _context.SaveChangesAsync();
        return membershipPlans;
    }

    private async Task CreateGymMembershipsAsync(List<MembershipPlan> membershipPlans, string adminId)
    {
        var clients = await _context.Clients.ToListAsync();
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

        _context.GymMemberships.AddRange(gymMemberships);
        await _context.SaveChangesAsync();
    }

    private async Task CreateEmploymentsAsync(string managerId)
    {
        var employees = await _context.Employees.ToListAsync();
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

        _context.Employments.AddRange(employments);
        await _context.SaveChangesAsync();
    }

    private async Task CreateShiftsAsync(string managerId)
    {
        var employees = await _context.Employees.ToListAsync();
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

        _context.Shifts.AddRange(shifts);
        await _context.SaveChangesAsync();
    }

    private async Task<List<GroupTraining>> CreateGroupTrainingsAsync(List<TrainingType> trainingTypes, string trainerId)
    {
        var trainers = await _context.Employees.Where(e => e.Role == EmployeeRole.Trainer).ToListAsync();
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

        _context.GroupTrainings.AddRange(groupTrainings);
        await _context.SaveChangesAsync();
        await CreateGroupTrainingParticipationsAsync(groupTrainings, trainerId);
        return groupTrainings;
    }

    private async Task CreateGroupTrainingParticipationsAsync(List<GroupTraining> groupTrainings, string adminId)
    {
        var clients = await _context.Clients.ToListAsync();
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

        _context.GroupTrainingParticipations.AddRange(participations);
        await _context.SaveChangesAsync();
    }

    private async Task CreateIndividualTrainingsAsync(string trainerId)
    {
        var trainers = await _context.Employees.Where(e => e.Role == EmployeeRole.Trainer).ToListAsync();
        var clients = await _context.Clients.ToListAsync();
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

        _context.IndividualTrainings.AddRange(individualTrainings);
        await _context.SaveChangesAsync();
    }

    private async Task CreateTrainingPlansWithSessionsAndExercisesAsync(string trainerId)
    {
        var trainers = await _context.Employees.Where(e => e.Role == EmployeeRole.Trainer).ToListAsync();
        var clients = await _context.Clients.ToListAsync();
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

        _context.TrainingPlans.AddRange(trainingPlans);
        await _context.SaveChangesAsync();

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

        _context.TrainingPlanSessions.AddRange(sessions);
        await _context.SaveChangesAsync();

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

        _context.Exercises.AddRange(exercises);
        await _context.SaveChangesAsync();
    }

    private async Task CreatePaymentsAsync(string adminId)
    {
        var gymMemberships = await _context.GymMemberships.Include(gm => gm.MembershipPlan).ToListAsync();
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

        _context.Payments.AddRange(payments);
        await _context.SaveChangesAsync();
    }

    private async Task<string> GetUserIdByRole(UserRole role, int index = 0)
    {
        var roleName = role.ToString();

        var usersInRole = await (
            from user in _context.Users
            join userRole in _context.UserRoles
                on user.Id equals userRole.UserId
            join identityRole in _context.Roles
                on userRole.RoleId equals identityRole.Id
            where identityRole.Name == roleName
            orderby user.Email
            select user.Id
        ).ToListAsync();

        if (!usersInRole.Any())
            throw new InvalidOperationException(
                $"No users found with role '{roleName}'. Ensure Identity seeding runs before domain seeding.");

        if (index >= usersInRole.Count)
            throw new InvalidOperationException(
                $"Requested index {index} for role '{roleName}' but only {usersInRole.Count} user(s) exist.");

        return usersInRole[index];
    }
}