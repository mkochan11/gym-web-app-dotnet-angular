using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Application.WebModels.User;
using MediatR;

namespace GymWebApp.Application.CQRS.Users;

public static class GetUserById
{
    public class Handler : IRequestHandler<GetUserByIdQuery, UserWebModel?>
    {
        private readonly IUserRepository _userRepository;

        public Handler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserWebModel?> Handle(GetUserByIdQuery query, CancellationToken ct)
        {
            return await _userRepository.GetByIdAsync(query.Id);
        }
    }
}

public class GetUserByIdQuery : IRequest<UserWebModel?>
{
    public string Id { get; set; } = null!;
}
