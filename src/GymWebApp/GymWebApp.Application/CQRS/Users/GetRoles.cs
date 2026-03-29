using GymWebApp.Application.Common.Authorization;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Enums;
using MediatR;

namespace GymWebApp.Application.CQRS.Users;

public class GetRolesQuery : IRequest<List<string>>
{
    public string? CurrentUserRole { get; set; }
}

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
            var roleNames = roles.Select(r => r.ToString()).ToList();

            if (!string.IsNullOrEmpty(query.CurrentUserRole))
            {
                var manageableRoles = UserRolePolicy.GetManageableRoles(query.CurrentUserRole);
                roleNames = roleNames.Where(r => manageableRoles.Contains(r)).ToList();
            }

            return roleNames;
        }
    }
}
