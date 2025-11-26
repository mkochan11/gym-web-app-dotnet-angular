using GymWebApp.Data.Entities;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.Data.Repositories;

public class ClientRepository : Repository<Client>, IClientRepository
{
    public ClientRepository(ApplicationDbContext context) : base(context)
    {
    }
}