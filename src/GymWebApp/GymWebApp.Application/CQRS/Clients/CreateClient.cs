using FluentValidation;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.Client;
using GymWebApp.Application.WebModels.User;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using MediatR;
using ValidationException = GymWebApp.Application.Common.Exceptions.ValidationException;

namespace GymWebApp.Application.CQRS.Clients;

public static class CreateClient
{
    public class Handler : IRequestHandler<CreateClientCommand, ClientUserWebModel>
    {
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;

        public Handler(
            IUserRepository userRepository,
            IClientRepository clientRepository)
        {
            _userRepository = userRepository;
            _clientRepository = clientRepository;
        }

        public async Task<ClientUserWebModel> Handle(CreateClientCommand command, CancellationToken ct)
        {
            var existingUser = await _userRepository.GetByEmailAsync(command.Email);
            if (existingUser != null)
            {
                throw new ValidationException("Email already exists", new Dictionary<string, string[]>
                {
                    { "Email", new[] { "Email is already registered" } }
                });
            }

            var password = string.IsNullOrEmpty(command.Password)
                ? GenerateSecurePassword()
                : command.Password;

            var model = new CreateClientWebModel
            {
                Email = command.Email,
                Password = password,
                FirstName = command.FirstName,
                LastName = command.LastName,
                PhoneNumber = command.PhoneNumber,
                DateOfBirth = command.DateOfBirth
            };

            var result = await _userRepository.CreateAsync(new CreateUserWebModel
            {
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Role = UserRole.Client.ToString()
            });

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

            var client = new Client
            {
                AccountId = createdUser.Id,
                Name = command.FirstName,
                Surname = command.LastName,
                DateOfBirth = command.DateOfBirth,
                RegistrationDate = DateTime.UtcNow
            };

            await _clientRepository.AddAsync(client);
            await _clientRepository.SaveChangesAsync();

            var userModel = await _userRepository.GetByIdAsync(createdUser.Id);
            return new ClientUserWebModel
            {
                Id = userModel!.Id,
                Email = userModel.Email,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                PhoneNumber = userModel.PhoneNumber,
                DateOfBirth = client.DateOfBirth,
                TemporaryPassword = password,
                CreatedAt = userModel.CreatedAt
            };
        }

        private static string GenerateSecurePassword()
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string special = "!@#$%^&*";
            const string allChars = upperChars + lowerChars + numbers + special;
            
            var random = new Random();
            var password = new char[12];
            
            password[0] = upperChars[random.Next(upperChars.Length)];
            password[1] = lowerChars[random.Next(lowerChars.Length)];
            password[2] = numbers[random.Next(numbers.Length)];
            password[3] = special[random.Next(special.Length)];
            
            for (int i = 4; i < 12; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }
            
            return new string(password.OrderBy(x => random.Next()).ToArray());
        }
    }

    public class Validator : AbstractValidator<CreateClientCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

            RuleFor(x => x.Password)
                .Must(BeValidPasswordOrEmpty)
                .WithMessage("Password must be at least 8 characters and contain uppercase, lowercase, number, and special character");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }

        private bool BeValidPasswordOrEmpty(string? password)
        {
            if (string.IsNullOrEmpty(password))
                return true;

            if (password.Length < 8)
                return false;

            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasNumber = password.Any(char.IsDigit);
            var hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            return hasUpper && hasLower && hasNumber && hasSpecial;
        }
    }
}

public class CreateClientCommand : IRequest<ClientUserWebModel>
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
