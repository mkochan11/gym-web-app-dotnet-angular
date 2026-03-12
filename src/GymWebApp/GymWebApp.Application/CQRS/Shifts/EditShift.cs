using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Enums;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.Shifts;

public static class EditShift
{
    public class Handler : IRequestHandler<EditShiftCommand>
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public Handler(
            IShiftRepository shiftRepository,
            IEmployeeRepository employeeRepository)
        {
            _shiftRepository = shiftRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task Handle(EditShiftCommand command, CancellationToken ct)
        {
            var shift = await _shiftRepository.GetByIdAsync(command.Id)
                ?? throw new NotFoundException("Shift", command.Id);

            var statuses = shift.GetShiftStatuses().ToArray();
            
            if (statuses.Contains(EventStatus.Cancelled) || statuses.Contains(EventStatus.Completed) || statuses.Contains(EventStatus.Ongoing))
            {
                var statusText = string.Join(", ", statuses.Select(s => s.ToString().ToLower()));
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Id", new[] { $"Cannot edit a {statusText} shift." } }
                });
            }

            var employee = await _employeeRepository.GetByIdWithEmploymentsAsync(command.EmployeeId)
                ?? throw new NotFoundException("Employee", command.EmployeeId);

            if (!employee.IsEmployeeActive())
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "EmployeeId", new[] { "Employee will not be active." } }
                });
            }

            var hasOverlap = await _shiftRepository.ExistsOverlappingExcludingAsync(
                command.EmployeeId,
                command.StartTime.ToUniversalTime(),
                command.EndTime.ToUniversalTime(),
                command.Id,
                ct);

            if (hasOverlap)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "StartTime", new[] { "Employee has an overlapping shift in this time range." } }
                });
            }

            shift.EmployeeId = command.EmployeeId;
            shift.StartTime = command.StartTime.ToUniversalTime();
            shift.EndTime = command.EndTime.ToUniversalTime();
            shift.UpdatedById = command.UpdatedById;
            shift.UpdatedAt = DateTime.UtcNow;

            await _shiftRepository.SaveChangesAsync();
        }
    }

    public class Validator : AbstractValidator<EditShiftCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.EmployeeId)
                .GreaterThan(0);

            RuleFor(x => x.StartTime)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Shift date must be in the future");

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.UpdatedById)
                .NotEmpty();
        }
    }
}

public class EditShiftCommand : IRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public string UpdatedById { get; set; } = string.Empty;
}
