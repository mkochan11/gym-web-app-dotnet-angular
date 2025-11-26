using GymWebApp.Data.Entities;
using GymWebApp.Data.Repositories.Interfaces;

namespace GymWebApp.Data.Repositories;

public class TrainingTypeRepository : Repository<TrainingType>, ITrainingTypeRepository
{
    public TrainingTypeRepository(ApplicationDbContext context) : base(context)
    {
    }
}