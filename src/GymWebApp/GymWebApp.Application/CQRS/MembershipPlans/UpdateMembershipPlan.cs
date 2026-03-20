using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.MembershipPlan;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.MembershipPlans;

public static class UpdateMembershipPlan
{
    public class Handler : IRequestHandler<UpdateMembershipPlanCommand, MembershipPlanWebModel>
    {
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public Handler(IMembershipPlanRepository membershipPlanRepository)
        {
            _membershipPlanRepository = membershipPlanRepository;
        }

        public async Task<MembershipPlanWebModel> Handle(UpdateMembershipPlanCommand command, CancellationToken ct)
        {
            var plan = await _membershipPlanRepository.GetByIdAsync(command.Id);
            if (plan == null)
            {
                throw new NotFoundException("MembershipPlan", command.Id);
            }

            var allPlans = await _membershipPlanRepository.GetAllAsync();
            if (allPlans.Any(p => p.Type == command.Type && p.Id != command.Id && p.IsActive))
            {
                throw new ValidationException("A membership plan with this type already exists", new Dictionary<string, string[]>
                {
                    { "Type", new[] { "A membership plan with this type already exists" } }
                });
            }

            plan.Type = command.Type;
            plan.Description = command.Description;
            plan.Price = command.Price;
            plan.DurationTime = command.DurationTime;
            plan.DurationInMonths = command.DurationInMonths;
            plan.CanReserveTrainings = command.CanReserveTrainings;
            plan.CanAccessGroupTraining = command.CanAccessGroupTraining;
            plan.CanAccessPersonalTraining = command.CanAccessPersonalTraining;
            plan.CanReceiveTrainingPlans = command.CanReceiveTrainingPlans;
            plan.MaxTrainingsPerMonth = command.MaxTrainingsPerMonth;
            plan.IsActive = command.IsActive;
            plan.UpdatedAt = DateTime.UtcNow;
            plan.UpdatedById = command.UpdatedById;

            _membershipPlanRepository.Update(plan);
            await _membershipPlanRepository.SaveChangesAsync();

            return plan.ToMembershipPlanWebModel();
        }
    }

    public class Validator : AbstractValidator<UpdateMembershipPlanCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid plan ID");

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

public class UpdateMembershipPlanCommand : IRequest<MembershipPlanWebModel>
{
    public int Id { get; set; }
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
    public bool IsActive { get; set; }
    public string? UpdatedById { get; set; }
}
