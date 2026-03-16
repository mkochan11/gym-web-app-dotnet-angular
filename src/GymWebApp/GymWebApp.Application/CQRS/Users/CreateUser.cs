using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using GymWebApp.Domain.Enums;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.Users;

public static class CreateUser
{
    public class Handler : IRequestHandler<CreateUserCommand, UserWebModel>
    {
        private readonly IUserRepository _userRepository;

        public Handler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserWebModel> Handle(CreateUserCommand command, CancellationToken ct)
        {
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

            var userModel = await _userRepository.GetByIdAsync(createdUser.Id);
            return userModel!;
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

public class CreateUserCommand : IRequest<UserWebModel>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = null!;
}
