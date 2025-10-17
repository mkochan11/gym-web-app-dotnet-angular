using GymWebApp.Data.Entities;
using GymWebApp.Data.Entities.Abstract;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Domain entities
        public DbSet<Client> Clients { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }

        // Training entities
        public DbSet<IndividualTraining> IndividualTrainings { get; set; }
        public DbSet<GroupTraining> GroupTrainings { get; set; }
        public DbSet<GroupTrainingParticipation> GroupTrainingParticipations { get; set; }
        public DbSet<TrainingType> TrainingTypes { get; set; }
        public DbSet<TrainingPlan> TrainingPlans { get; set; }
        public DbSet<Exercise> Exercises { get; set; }

        // Membership and payment entities
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<GymMembership> GymMemberships { get; set; }
        public DbSet<Payment> Payments { get; set; }

        // Other entities
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Employment> Employments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Client>()
                .HasMany(c => c.GymMemberships)
                .WithOne(gm => gm.Client)
                .HasForeignKey(gm => gm.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Client>()
                .HasMany(c => c.IndividualTrainings)
                .WithOne(it => it.Client)
                .HasForeignKey(it => it.ClientId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Client>()
                .HasMany(c => c.GroupTrainingsParticipations)
                .WithOne(gtp => gtp.Client)
                .HasForeignKey(gtp => gtp.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Client>()
                .HasMany(c => c.TrainingPlans)
                .WithOne(tp => tp.Client)
                .HasForeignKey(tp => tp.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure GroupTraining relationships
            builder.Entity<GroupTraining>()
                .HasMany(gt => gt.Participations)
                .WithOne(gtp => gtp.GroupTraining)
                .HasForeignKey(gtp => gtp.GroupTrainingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GroupTraining>()
                .HasOne(gt => gt.TrainingType)
                .WithMany()
                .HasForeignKey(gt => gt.TrainingTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure TrainingPlan relationships
            builder.Entity<TrainingPlan>()
                .HasMany(tp => tp.Exercises)
                .WithOne(e => e.TrainingPlan)
                .HasForeignKey(e => e.TrainingPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure MembershipPlan relationships
            builder.Entity<MembershipPlan>()
                .HasMany(mp => mp.GymMemberships)
                .WithOne(gm => gm.MembershipPlan)
                .HasForeignKey(gm => gm.MembershipPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure GymMembership relationships
            builder.Entity<GymMembership>()
                .HasMany(gm => gm.Payments)
                .WithOne(p => p.GymMembership)
                .HasForeignKey(p => p.GymMembershipId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Receptionist relationships
            builder.Entity<Receptionist>()
                .HasMany(r => r.Shifts)
                .WithOne(s => s.Receptionist)
                .HasForeignKey(s => s.ReceptionistId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Employment relationships
            builder.Entity<Employment>()
                .HasOne(e => e.Employee)
                .WithMany(emp => emp.Employments)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes for better performance
            builder.Entity<User>()
                .HasIndex(u => u.AccountId)
                .IsUnique();

            builder.Entity<GymMembership>()
                .HasIndex(gm => new { gm.ClientId, gm.StartDate });

            builder.Entity<Payment>()
                .HasIndex(p => p.TransactionId)
                .IsUnique();

            builder.Entity<GroupTrainingParticipation>()
                .HasIndex(gtp => new { gtp.GroupTrainingId, gtp.ClientId })
                .IsUnique();

            builder.Entity<Employment>()
                .HasIndex(e => new { e.EmployeeId, e.StartDate });

            // Configure decimal precision
            builder.Entity<Employment>()
                .Property(e => e.Salary)
                .HasPrecision(10, 2);

            builder.Entity<MembershipPlan>()
                .Property(mp => mp.Price)
                .HasPrecision(10, 2);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);
        }
    }
}
