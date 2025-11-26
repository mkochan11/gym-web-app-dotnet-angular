using GymWebApp.Data.DTOs;
using GymWebApp.Data.Entities;
using GymWebApp.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymWebApp.Data.Repositories;

public class IndividualTrainingRepository : Repository<IndividualTraining>, IIndividualTrainingRepository
{
    public IndividualTrainingRepository(ApplicationDbContext context) : base(context)
    {
        
    }

    public async Task<IEnumerable<IndividualTraining>> GetAllIndividualTrainingsWithTrainersAsync() =>
        await _context.IndividualTrainings
            .Include(it => it.Trainer)
            .Include(it => it.Client)
            .Where(it => !it.Removed)
            .ToListAsync();

    public async Task<IEnumerable<IndividualTraining>> GetFilteredIndividualTrainingsAsync(IndividualTrainingFiltersDto filters)
    {
        var query = _context.IndividualTrainings
            .Include(it => it.Trainer)
            .Include(it => it.Client)
            .AsQueryable();

        if (filters.StartDate.HasValue)
        {
            query = query.Where(it => it.Date >= filters.StartDate.Value);
        }

        if (filters.EndDate.HasValue)
        {
            query = query.Where(it => it.Date <= filters.EndDate.Value);
        }

        if (filters.TrainersIds?.Any() == true)
        {
            query = query.Where(it => filters.TrainersIds.Contains(it.TrainerId));
        }

        if (filters.ClientsIds?.Any() == true)
        {
            query = query.Where(it => it.ClientId != null && filters.ClientsIds.Contains(it.ClientId.Value));
        }

        return await query.ToListAsync();
    }
}