using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.Users;

public static class GetRoles
{
    public class Handler : IRequestHandler<GetRolesQuery, List<string>>
    {
        private readonly IUserRepository _userRepository;

        public Handler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<string>> Handle(GetRolesQuery query, CancellationToken ct)
        {
            var roles = await _userRepository.GetAllRolesAsync();
            return roles.Select(r => r.ToString()).ToList();
        }
    }
}

public class GetRolesQuery : IRequest<List<string>>
{
}
