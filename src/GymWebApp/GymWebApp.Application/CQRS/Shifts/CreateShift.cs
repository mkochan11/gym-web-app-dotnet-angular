using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Extensions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.Shifts;

public static class CreateShift
{    
    public class Handler : IRequestHandler<CreateShiftCommand, int>
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
    
        public async Task<int> Handle(CreateShiftCommand command, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByIdWithEmploymentsAsync(command.EmployeeId)
                ?? throw new NotFoundException("Employee", command.EmployeeId);

            if (!employee.IsEmployeeActive())
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "EmployeeId", new[] { "Employee will not be active." } }
                });

            //TODO: Check for overlapping shifts for the same employee

            var shift = new Shift
            {
                EmployeeId = command.EmployeeId,
                StartTime = command.StartTime.ToUniversalTime(),
                EndTime = command.EndTime.ToUniversalTime(),
                CreatedById = command.CreatedById,
                CreatedAt = DateTime.UtcNow
            };
    
            await _shiftRepository.AddAsync(shift);
            await _shiftRepository.SaveChangesAsync();
    
            return shift.Id;
        }
    }

    public class Validator : AbstractValidator<CreateShiftCommand>
    {
        public Validator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0);

            RuleFor(x => x.StartTime)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Shift date must be in the future");

            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .WithMessage("End date must be after start date");

            //TODO: check if date is within working hours of the gym

            RuleFor(x => x.CreatedById)
                .NotEmpty();
        }
    }
}

public class CreateShiftCommand : IRequest<int>
{
    public int EmployeeId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string CreatedById { get; set; } = string.Empty;
}