using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.Interfaces.Services;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.GroupTrainings;

public static class EditGroupTraining
{
    public class Handler : IRequestHandler<EditGroupTrainingCommand>
    {
        private readonly IGroupTrainingRepository _groupTrainingRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITrainingTypeRepository _trainingTypeRepository;
        private readonly ITrainerService _trainerService;

        public Handler(
            IGroupTrainingRepository groupTrainingRepository,
            IEmployeeRepository employeeRepository,
            ITrainingTypeRepository trainingTypeRepository,
            ITrainerService trainerService)
        {
            _groupTrainingRepository = groupTrainingRepository;
            _employeeRepository = employeeRepository;
            _trainingTypeRepository = trainingTypeRepository;
            _trainerService = trainerService;
        }

        public async Task Handle(EditGroupTrainingCommand command, CancellationToken ct)
        {
            var training = await _groupTrainingRepository.GetByIdAsync(command.Id)
                ?? throw new NotFoundException("Group Training", command.Id);

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

            if (trainer.Role != EmployeeRole.Trainer)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "TrainerId", new[] { "Selected employee is not a trainer." } }
                });
            }

            if (!trainer.IsEmployeeActive())
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "TrainerId", new[] { "Trainer will not be active." } }
                });
            }

            var trainingType = await _trainingTypeRepository.GetByIdAsync(command.TrainingTypeId);
            if (trainingType == null)
            {
                throw new NotFoundException("Training Type", command.TrainingTypeId);
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
            training.TrainingTypeId = command.TrainingTypeId;
            training.MaxParticipantNumber = command.MaxParticipantNumber;
            training.DifficultyLevel = command.DifficultyLevel;
            training.StartTime = command.StartDate.ToUniversalTime();
            training.EndTime = command.EndDate.ToUniversalTime();
            training.Description = command.Description ?? string.Empty;
            training.Notes = command.Notes ?? string.Empty;
            training.UpdatedById = command.UpdatedById;
            training.UpdatedAt = DateTime.UtcNow;

            await _groupTrainingRepository.SaveChangesAsync();
        }
    }

    public class Validator : AbstractValidator<EditGroupTrainingCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.TrainerId)
                .GreaterThan(0);

            RuleFor(x => x.TrainingTypeId)
                .GreaterThan(0);

            RuleFor(x => x.MaxParticipantNumber)
                .GreaterThan(0);

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

public class EditGroupTrainingCommand : IRequest
{
    public int Id { get; set; }
    public int TrainerId { get; init; }
    public int TrainingTypeId { get; init; }
    public int MaxParticipantNumber { get; init; }
    public int DifficultyLevel { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string? Description { get; init; }
    public string? Notes { get; init; }
    public string UpdatedById { get; set; } = string.Empty;
}
