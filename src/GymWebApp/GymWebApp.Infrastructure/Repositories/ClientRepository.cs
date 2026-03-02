using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Infrastructure.Repositories;

public class ClientRepository : Repository<Client>, IClientRepository
{
    public ClientRepository(ApplicationDbContext context) : base(context)
    {
    }
}