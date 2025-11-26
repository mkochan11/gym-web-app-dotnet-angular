using GymWebApp.Data.DTOs;
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

    public async Task<IEnumerable<GroupTraining>> GetFilteredGroupTrainingsAsync(GroupTrainingFiltersDto filters)
    {
        var query = _context.GroupTrainings
            .Include(gt => gt.Trainer)
            .Include(gt => gt.TrainingType)
            .AsQueryable();

        if (filters.StartDate.HasValue)
        {
            query = query.Where(gt => gt.Date >= filters.StartDate.Value);
        }

        if (filters.EndDate.HasValue)
        {
            query = query.Where(gt => gt.Date <= filters.EndDate.Value);
        }

        if (filters.TrainerIds?.Any() == true)
        {
            query = query.Where(gt => filters.TrainerIds.Contains(gt.TrainerId));
        }

        if (filters.TrainingTypeIds?.Any() == true)
        {
            query = query.Where(gt => filters.TrainingTypeIds.Contains(gt.TrainingTypeId));
        }

        return await query.ToListAsync();
    }
}
