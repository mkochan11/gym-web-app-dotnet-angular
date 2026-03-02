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

public static class CreateGroupTraining
{
    public class Handler : IRequestHandler<CreateGroupTrainingCommand, int>
    {
        private readonly IGroupTrainingRepository _groupTrainingRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITrainingTypeRepository _trainingTypeRepository;
        private readonly ITrainerService _trainerService;

        public Handler(IGroupTrainingRepository groupTrainingRepository,
            IEmployeeRepository employeeRepository,
            ITrainingTypeRepository trainingTypeRepository,
            ITrainerService trainerService)
        {
            _groupTrainingRepository = groupTrainingRepository;
            _employeeRepository = employeeRepository;
            _trainingTypeRepository = trainingTypeRepository;
            _trainerService = trainerService;
        }

        public async Task<int> Handle(CreateGroupTrainingCommand command, CancellationToken ct)
        {
            var training = new GroupTraining
            {
                TrainerId = command.TrainerId,
                MaxParticipantNumber = command.MaxParticipantNumber,
                TrainingTypeId = command.TrainingTypeId,
                DifficultyLevel = command.DifficultyLevel,
                Date = command.StartDate.ToUniversalTime(),
                Duration = TimeSpan.FromMinutes((command.EndDate - command.StartDate).TotalMinutes),
                Description = command.Description ?? string.Empty,
                Notes = command.Notes,
                CreatedById = command.CreatedById,
                CreatedAt = DateTime.UtcNow
            };

            var trainer = await _employeeRepository.GetByIdAsync(command.TrainerId);
            if (trainer == null || trainer.Role != EmployeeRole.Trainer)
                throw new NotFoundException("Trainer not found");

            /*if (!trainer.IsEmployeeActive())
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "TrainerId", new[] { "Trainer will not be active." } }
                });

            var isTrainerAvailable = await _trainerService.IsAvailableAsync(command.TrainerId, command.StartDate, command.EndDate, ct);

            if (!isTrainerAvailable)
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "StartDate", new[] { "Trainer is not available in this time range." } }
                });*/

            _ = await _trainingTypeRepository.GetByIdAsync(command.TrainingTypeId) ?? throw new NotFoundException("Training type not found");

            await _groupTrainingRepository.AddAsync(training);
            await _groupTrainingRepository.SaveChangesAsync();

            return training.Id;
        }

        public class Validator : AbstractValidator<CreateGroupTrainingCommand>
        {
            public Validator()
            {
                RuleFor(x => x.MaxParticipantNumber).GreaterThan(0);
                RuleFor(x => x.TrainerId)
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

                RuleFor(x => x.CreatedById)
                    .NotEmpty();
            }
        }
    }
}

public class CreateGroupTrainingCommand : IRequest<int>
{
    public int TrainerId { get; init; }
    public int MaxParticipantNumber { get; init; }
    public int TrainingTypeId { get; init; }
    public int DifficultyLevel { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string? Description { get; init; }
    public string? Notes { get; init; }
    public string CreatedById { get; set; } = string.Empty;
}