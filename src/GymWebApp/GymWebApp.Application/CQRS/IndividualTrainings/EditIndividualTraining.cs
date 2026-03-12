using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.Interfaces.Services;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.IndividualTrainings;

public static class EditIndividualTraining
{
    public class Handler : IRequestHandler<EditIndividualTrainingCommand>
    {
        private readonly IIndividualTrainingRepository _trainingRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITrainerService _trainerService;

        public Handler(
            IIndividualTrainingRepository trainingRepository,
            IEmployeeRepository employeeRepository,
            ITrainerService trainerService)
        {
            _trainingRepository = trainingRepository;
            _employeeRepository = employeeRepository;
            _trainerService = trainerService;
        }

        public async Task Handle(EditIndividualTrainingCommand command, CancellationToken ct)
        {
            var training = await _trainingRepository.GetByIdAsync(command.Id)
                ?? throw new NotFoundException("Individual Training", command.Id);

            var statuses = training.GetTrainingStatuses().ToArray();
            if (statuses.Contains(EventStatus.Cancelled) || statuses.Contains(EventStatus.Completed) || statuses.Contains(EventStatus.Ongoing))
            {
                var statusText = string.Join(", ", statuses.Select(s => s.ToString().ToLower()));
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Id", new[] { $"Cannot edit a {statusText} training." } }
                });
            }

            var trainer = await _employeeRepository.GetByIdWithEmploymentsAsync(command.TrainerId)
                ?? throw new NotFoundException("Trainer", command.TrainerId);

            if (!trainer.IsEmployeeActive())
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "TrainerId", new[] { "Trainer will not be active." } }
                });
            }

            var isTrainerAvailable = await _trainerService.IsAvailableExcludingAsync(
                command.TrainerId,
                command.StartDate,
                command.EndDate,
                command.Id,
                ct);

            if (!isTrainerAvailable)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "StartDate", new[] { "Trainer is not available in this time range." } }
                });
            }

            training.TrainerId = command.TrainerId;
            training.ClientId = command.ClientId;
            training.StartTime = command.StartDate.ToUniversalTime();
            training.EndTime = command.EndDate.ToUniversalTime();
            training.Description = command.Description ?? string.Empty;
            training.Notes = command.Notes ?? string.Empty;
            training.UpdatedById = command.UpdatedById;
            training.UpdatedAt = DateTime.UtcNow;

            await _trainingRepository.SaveChangesAsync();
        }
    }

    public class Validator : AbstractValidator<EditIndividualTrainingCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.TrainerId)
                .GreaterThan(0);

            RuleFor(x => x.ClientId)
                .GreaterThan(0)
                .When(x => x.ClientId.HasValue);

            RuleFor(x => x.StartDate)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Training date must be in the future");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date");

            RuleFor(x => x)
                .Must(x =>
                {
                    var duration = x.EndDate - x.StartDate;
                    return duration.TotalMinutes >= 15 &&
                           duration.TotalMinutes <= 180;
                })
                .WithMessage("Duration must be between 15 and 180 minutes");

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.Notes)
                .MaximumLength(1000);

            RuleFor(x => x.UpdatedById)
                .NotEmpty();
        }
    }
}

public record EditIndividualTrainingCommand(
    int Id,
    int TrainerId,
    int? ClientId,
    DateTime StartDate,
    DateTime EndDate,
    string? Description,
    string? Notes,
    string UpdatedById
) : IRequest;
