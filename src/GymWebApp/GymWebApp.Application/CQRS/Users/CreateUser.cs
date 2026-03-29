using FluentValidation;
using GymWebApp.Application.Common.Authorization;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.Users;

public class CreateUserCommand : IRequest<UserWebModel>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = null!;
    public string? CurrentUserRole { get; set; }
}

public static class CreateUser
{
    public class Handler : IRequestHandler<CreateUserCommand, UserWebModel>
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

        public async Task<UserWebModel> Handle(CreateUserCommand command, CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(command.CurrentUserRole))
            {
                if (!UserRolePolicy.CanManageRole(command.CurrentUserRole, command.Role))
                {
                    throw new ValidationException($"Role '{command.Role}' cannot be assigned by {command.CurrentUserRole}", new Dictionary<string, string[]>
                    {
                        { "Role", new[] { $"You don't have permission to create users with role '{command.Role}'" } }
                    });
                }
            }

            var existingUser = await _userRepository.GetByEmailAsync(command.Email);
            if (existingUser != null)
            {
                throw new ValidationException("Email already exists", new Dictionary<string, string[]>
                {
                    { "Email", new[] { "Email is already registered" } }
                });
            }

            var model = new CreateUserWebModel
            {
                Email = command.Email,
                Password = command.Password,
                FirstName = command.FirstName,
                LastName = command.LastName,
                PhoneNumber = command.PhoneNumber,
                Role = command.Role
            };

            var result = await _userRepository.CreateAsync(model);

            if (result.HasValue)
            {
                throw new ValidationException("User creation failed", new Dictionary<string, string[]>
                {
                    { "CreateUser", new[] { result.Value.Error } }
                });
            }

            var createdUser = await _userRepository.GetByEmailAsync(command.Email);
            if (createdUser == null)
            {
                throw new InvalidOperationException("Failed to retrieve created user");
            }

            await CreateRelatedEntityAsync(createdUser.Id, command);

            if (Enum.TryParse<UserRole>(command.Role, out var role))
            {
                if (role == UserRole.Client)
                {
                    await _clientRepository.SaveChangesAsync();
                }
                else if (IsEmployeeRole(role))
                {
                    await _employeeRepository.SaveChangesAsync();
                }
            }

            var userModel = await _userRepository.GetByIdAsync(createdUser.Id);
            return userModel!;
        }

        private async Task CreateRelatedEntityAsync(string userId, CreateUserCommand command)
        {
            if (Enum.TryParse<UserRole>(command.Role, out var userRole))
            {
                if (userRole == UserRole.Client)
                {
                    var client = new Client
                    {
                        AccountId = userId,
                        Name = command.FirstName,
                        Surname = command.LastName,
                        RegistrationDate = DateTime.UtcNow
                    };
                    await _clientRepository.AddAsync(client);
                }
                else if (IsEmployeeRole(userRole))
                {
                    var employeeRole = MapToEmployeeRole(userRole);
                    var employee = new Employee
                    {
                        AccountId = userId,
                        Name = command.FirstName,
                        Surname = command.LastName,
                        Role = employeeRole,
                        RegistrationDate = DateTime.UtcNow
                    };
                    await _employeeRepository.AddAsync(employee);
                }
            }
        }

        private static bool IsEmployeeRole(UserRole role)
        {
            return role == UserRole.Owner ||
                   role == UserRole.Manager ||
                   role == UserRole.Trainer ||
                   role == UserRole.Receptionist;
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

    public class Validator : AbstractValidator<CreateUserCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

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
