using FluentValidation;
using GymWebApp.Application.Common.Authorization;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.Users;

public class DeleteUserCommand : IRequest<bool>
{
    public string UserId { get; set; } = null!;
    public string? CurrentUserRole { get; set; }
}

public static class DeleteUser
{
    public class Handler : IRequestHandler<DeleteUserCommand, bool>
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

        public async Task<bool> Handle(DeleteUserCommand command, CancellationToken ct)
        {
            var existingUser = await _userRepository.GetByIdAsync(command.UserId);
            if (existingUser == null)
            {
                throw new NotFoundException("User", command.UserId);
            }

            if (!string.IsNullOrEmpty(command.CurrentUserRole))
            {
                if (!UserRolePolicy.CanManageRole(command.CurrentUserRole, existingUser.Role))
                {
                    throw new ValidationException("Cannot delete this user", new Dictionary<string, string[]>
                    {
                        { "DeleteUser", new[] { "You don't have permission to delete users with this role" } }
                    });
                }
            }

            var currentUserId = await _userRepository.GetCurrentUserIdAsync();
            if (!string.IsNullOrEmpty(currentUserId) && currentUserId == command.UserId)
            {
                throw new ValidationException("Cannot delete your own account", new Dictionary<string, string[]>
                {
                    { "DeleteUser", new[] { "You cannot delete your own account" } }
                });
            }

            await DeleteRelatedEntitiesAsync(command.UserId, existingUser.Role);

            var result = await _userRepository.DeleteAsync(command.UserId);
            return result;
        }

        private async Task DeleteRelatedEntitiesAsync(string userId, string userRole)
        {
            if (userRole == "Client")
            {
                await _clientRepository.SoftDeleteByAccountIdAsync(userId);
                await _clientRepository.SaveChangesAsync();
            }
            else if (IsEmployeeRole(userRole))
            {
                await _employeeRepository.SoftDeleteByAccountIdAsync(userId);
                await _employeeRepository.SaveChangesAsync();
            }
        }

        private static bool IsEmployeeRole(string role)
        {
            return role == "Owner" ||
                   role == "Manager" ||
                   role == "Trainer" ||
                   role == "Receptionist";
        }
    }

    public class Validator : AbstractValidator<DeleteUserCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .Must(BeAValidId).WithMessage("User ID must not be whitespace");
        }

        private bool BeAValidId(string id)
        {
            return !string.IsNullOrWhiteSpace(id);
        }
    }
}
