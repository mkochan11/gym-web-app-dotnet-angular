using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.Users;

public static class UpdateUser
{
    public class Handler : IRequestHandler<UpdateUserCommand, UserWebModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public Handler(
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IEmployeeRepository employeeRepository)
        {
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<UserWebModel> Handle(UpdateUserCommand command, CancellationToken ct)
        {
            var existingUser = await _userRepository.GetByIdAsync(command.Id);
            if (existingUser == null)
            {
                throw new NotFoundException("User", command.Id);
            }

            if (!string.Equals(existingUser.Email, command.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailExists = await _userRepository.EmailExistsAsync(command.Email);
                if (emailExists)
                {
                    throw new ValidationException("Email already exists", new Dictionary<string, string[]>
                    {
                        { "Email", new[] { "Email is already registered" } }
                    });
                }
            }

            var oldRole = existingUser.Role;
            var newRole = command.Role;

            var updateError = await _userRepository.UpdateAsync(
                command.Id,
                command.Email,
                command.FirstName,
                command.LastName,
                command.PhoneNumber,
                command.Role
            );

            if (updateError != null)
            {
                throw new ValidationException("User update failed", new Dictionary<string, string[]>
                {
                    { "UpdateUser", new[] { updateError } }
                });
            }

            await HandleRelatedEntityChangesAsync(command.Id, oldRole, newRole, command.FirstName, command.LastName);

            await _clientRepository.SaveChangesAsync();
            await _employeeRepository.SaveChangesAsync();

            var updatedUser = await _userRepository.GetByIdAsync(command.Id);
            return updatedUser!;
        }

        private async Task HandleRelatedEntityChangesAsync(
            string userId, 
            string oldRole, 
            string newRole, 
            string firstName, 
            string lastName)
        {
            var oldIsEmployee = IsEmployeeRole(oldRole);
            var newIsEmployee = IsEmployeeRole(newRole);
            var oldIsClient = oldRole == "Client";
            var newIsClient = newRole == "Client";

            if (oldIsClient && !newIsClient)
            {
                var client = await _clientRepository.GetByAccountIdAsync(userId);
                if (client != null)
                {
                    _clientRepository.Remove(client);
                }
            }
            else if (oldIsClient && newIsClient)
            {
                var client = await _clientRepository.GetByAccountIdAsync(userId);
                if (client != null)
                {
                    client.Name = firstName;
                    client.Surname = lastName;
                    _clientRepository.Update(client);
                }
            }
            else if (!oldIsClient && newIsClient)
            {
                var client = new Client
                {
                    AccountId = userId,
                    Name = firstName,
                    Surname = lastName,
                    RegistrationDate = DateTime.UtcNow
                };
                await _clientRepository.AddAsync(client);
            }

            if (oldIsEmployee && !newIsEmployee)
            {
                var employee = await _employeeRepository.GetByAccountIdAsync(userId);
                if (employee != null)
                {
                    _employeeRepository.Remove(employee);
                }
            }
            else if (oldIsEmployee && newIsEmployee)
            {
                var employee = await _employeeRepository.GetByAccountIdAsync(userId);
                if (employee != null)
                {
                    employee.Name = firstName;
                    employee.Surname = lastName;
                    if (Enum.TryParse<UserRole>(newRole, out var userRole))
                    {
                        employee.Role = MapToEmployeeRole(userRole);
                    }
                    _employeeRepository.Update(employee);
                }
            }
            else if (!oldIsEmployee && newIsEmployee)
            {
                if (Enum.TryParse<UserRole>(newRole, out var userRole))
                {
                    var employee = new Employee
                    {
                        AccountId = userId,
                        Name = firstName,
                        Surname = lastName,
                        Role = MapToEmployeeRole(userRole),
                        RegistrationDate = DateTime.UtcNow
                    };
                    await _employeeRepository.AddAsync(employee);
                }
            }
        }

        private static bool IsEmployeeRole(string role)
        {
            return role == "Owner" || 
                   role == "Manager" || 
                   role == "Trainer" || 
                   role == "Receptionist";
        }

        private static EmployeeRole MapToEmployeeRole(UserRole role)
        {
            return role switch
            {
                UserRole.Owner => EmployeeRole.Owner,
                UserRole.Manager => EmployeeRole.Manager,
                UserRole.Trainer => EmployeeRole.Trainer,
                UserRole.Receptionist => EmployeeRole.Receptionist,
                _ => throw new ArgumentException($"Cannot map {role} to EmployeeRole")
            };
        }
    }

    public class Validator : AbstractValidator<UpdateUserCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(BeAValidRole).WithMessage("Invalid role");
        }

        private bool BeAValidRole(string role)
        {
            return Enum.TryParse<UserRole>(role, out _);
        }
    }
}

public class UpdateUserCommand : IRequest<UserWebModel>
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = null!;
}
