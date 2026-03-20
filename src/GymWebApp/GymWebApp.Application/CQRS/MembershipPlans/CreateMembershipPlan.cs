using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.MembershipPlan;
using GymWebApp.Domain.Entities;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.MembershipPlans;

public static class CreateMembershipPlan
{
    public class Handler : IRequestHandler<CreateMembershipPlanCommand, MembershipPlanWebModel>
    {
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public Handler(IMembershipPlanRepository membershipPlanRepository)
        {
            _membershipPlanRepository = membershipPlanRepository;
        }

        public async Task<MembershipPlanWebModel> Handle(CreateMembershipPlanCommand command, CancellationToken ct)
        {
            var allPlans = await _membershipPlanRepository.GetAllAsync();
            if (allPlans.Any(p => p.Type == command.Type && p.IsActive))
            {
                throw new ValidationException("A membership plan with this type already exists", new Dictionary<string, string[]>
                {
                    { "Type", new[] { "A membership plan with this type already exists" } }
                });
            }

            var plan = new MembershipPlan
            {
                Type = command.Type,
                Description = command.Description,
                Price = command.Price,
                DurationTime = command.DurationTime,
                DurationInMonths = command.DurationInMonths,
                CanReserveTrainings = command.CanReserveTrainings,
                CanAccessGroupTraining = command.CanAccessGroupTraining,
                CanAccessPersonalTraining = command.CanAccessPersonalTraining,
                CanReceiveTrainingPlans = command.CanReceiveTrainingPlans,
                MaxTrainingsPerMonth = command.MaxTrainingsPerMonth,
                IsActive = command.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedById = command.CreatedById
            };

            await _membershipPlanRepository.AddAsync(plan);
            await _membershipPlanRepository.SaveChangesAsync();

            return plan.ToMembershipPlanWebModel();
        }
    }

    public class Validator : AbstractValidator<CreateMembershipPlanCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required")
                .MaximumLength(100).WithMessage("Type must not exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.DurationTime)
                .NotEmpty().WithMessage("Duration time is required")
                .MaximumLength(50).WithMessage("Duration time must not exceed 50 characters");

            RuleFor(x => x.DurationInMonths)
                .GreaterThan(0).WithMessage("Duration in months must be greater than 0");

            RuleFor(x => x.MaxTrainingsPerMonth)
                .GreaterThan(0).When(x => x.MaxTrainingsPerMonth.HasValue)
                .WithMessage("Max trainings per month must be greater than 0");
        }
    }
}

public class CreateMembershipPlanCommand : IRequest<MembershipPlanWebModel>
{
    public string Type { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string DurationTime { get; set; } = null!;
    public int DurationInMonths { get; set; }
    public bool CanReserveTrainings { get; set; }
    public bool CanAccessGroupTraining { get; set; }
    public bool CanAccessPersonalTraining { get; set; }
    public bool CanReceiveTrainingPlans { get; set; }
    public int? MaxTrainingsPerMonth { get; set; }
    public bool IsActive { get; set; } = true;
    public string CreatedById { get; set; } = string.Empty;
}
