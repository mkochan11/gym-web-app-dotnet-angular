using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using MediatR;

namespace GymWebApp.Application.CQRS.Users;

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
            return await _userRepository.GetAllAsync();
        }
    }
}

public class GetUsersQuery : IRequest<List<UserWebModel>>
{
}
