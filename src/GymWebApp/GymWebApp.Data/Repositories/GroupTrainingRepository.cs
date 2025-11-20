using GymWebApp.Data.Entities;
using GymWebApp.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymWebApp.Data.Repositories;

public class GroupTrainingRepository : Repository<GroupTraining>, IGroupTrainingRepository
{
    public GroupTrainingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<GroupTraining>> GetAllGroupTrainingsWithTrainersAsync() =>
        await _context.GroupTrainings
            .Include(gt => gt.Trainer)
            .Include(gt => gt.Participations)
            .Include(gt => gt.TrainingType)
            .Where(gt => !gt.Removed)
            .ToListAsync();

}
