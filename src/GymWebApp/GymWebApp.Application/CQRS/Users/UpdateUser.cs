using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using GymWebApp.Domain.Enums;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.Users;

public static class UpdateUser
{
    public class Handler : IRequestHandler<UpdateUserCommand, UserWebModel>
    {
        private readonly IUserRepository _userRepository;

        public Handler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

            var updatedUser = await _userRepository.GetByIdAsync(command.Id);
            return updatedUser!;
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
