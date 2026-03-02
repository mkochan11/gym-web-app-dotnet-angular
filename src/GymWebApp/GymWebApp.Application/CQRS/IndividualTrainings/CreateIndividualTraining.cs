using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.Interfaces.Services;
using GymWebApp.Domain.Entities;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.IndividualTrainings;

public static class CreateIndividualTraining
{
    public class Handler : IRequestHandler<CreateIndividualTrainingCommand, int>
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

        public async Task<int> Handle(CreateIndividualTrainingCommand command, CancellationToken ct)
        {
            var trainer = await _employeeRepository.GetByIdAsync(command.TrainerId)
                ?? throw new NotFoundException("Trainer", command.TrainerId);

            if (!trainer.IsEmployeeActive())
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "TrainerId", new[] { "Trainer will not be active." } }
                });

            var isTrainerAvailable = await _trainerService.IsAvailableAsync(command.TrainerId, command.StartDate, command.EndDate, ct);

            if (!isTrainerAvailable)
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "StartDate", new[] { "Trainer is not available in this time range." } }
                });

            var duration = command.EndDate - command.StartDate;

            var training = new IndividualTraining
            {
                TrainerId = command.TrainerId,
                Date = command.StartDate.ToUniversalTime(),
                Duration = duration,
                Description = command.Description ?? string.Empty,
                Notes = command.Notes ?? string.Empty,
                CreatedById = command.CreatedById,
                CreatedAt = DateTime.UtcNow
            };

            await _trainingRepository.AddAsync(training);
            await _trainingRepository.SaveChangesAsync();

            return training.Id;
        }
    }

    public class Validator : AbstractValidator<CreateIndividualTrainingCommand>
    {
        public Validator()
        {
            RuleFor(x => x.TrainerId)
                .GreaterThan(0);

            RuleFor(x => x.StartDate)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Training date must be in the future");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date");

            //TODO: check if date is within working hours of the gym

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

public class CreateIndividualTrainingCommand : IRequest<int>
{
    public int TrainerId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string? Description { get; init; }
    public string Notes { get; init; } = string.Empty;
    public string CreatedById { get; set; } = string.Empty;
}