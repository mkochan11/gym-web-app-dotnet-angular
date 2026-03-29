using GymWebApp.Application.Common.Authorization;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using MediatR;
using System.Security.Claims;

namespace GymWebApp.Application.CQRS.Users;

public class GetUsersQuery : IRequest<List<UserWebModel>>
{
    public string? CurrentUserRole { get; set; }
}

public static class GetUsers
{
    public class Handler : IRequestHandler<GetUsersQuery, List<UserWebModel>>
    {
        private readonly IUserRepository _userRepository;

        public Handler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserWebModel>> Handle(GetUsersQuery query, CancellationToken ct)
        {
            var users = await _userRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(query.CurrentUserRole))
            {
                var manageableRoles = UserRolePolicy.GetManageableRoles(query.CurrentUserRole);
                users = users.Where(u => manageableRoles.Contains(u.Role)).ToList();
            }

            return users;
        }
    }
}
