using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Infrastructure.Repositories;

public class TrainingTypeRepository : Repository<TrainingType>, ITrainingTypeRepository
{
    public TrainingTypeRepository(ApplicationDbContext context) : base(context)
    {
    }
}