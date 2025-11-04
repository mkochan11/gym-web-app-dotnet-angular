using System.Reflection.Emit;
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
        public DbSet<Employee> Employees { get; set; }

        // Training entities
        public DbSet<IndividualTraining> IndividualTrainings { get; set; }
        public DbSet<GroupTraining> GroupTrainings { get; set; }
        public DbSet<GroupTrainingParticipation> GroupTrainingParticipations { get; set; }
        public DbSet<TrainingType> TrainingTypes { get; set; }
        public DbSet<TrainingPlan> TrainingPlans { get; set; }
        public DbSet<TrainingPlanSession> TrainingPlanSessions { get; set; }
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

            foreach (var fk in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            builder.Entity<AuditableEntity>()
                .HasOne(a => a.CreatedBy)
                .WithMany()
                .HasForeignKey(a => a.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AuditableEntity>()
                .HasOne(a => a.UpdatedBy)
                .WithMany()
                .HasForeignKey(a => a.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
