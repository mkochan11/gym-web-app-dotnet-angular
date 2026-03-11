using GymWebApp.Application.DTOs;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymWebApp.Infrastructure.Repositories;

public class IndividualTrainingRepository(ApplicationDbContext context) : Repository<IndividualTraining>(context), IIndividualTrainingRepository
{
    public async Task<bool> ExistsOverlappingAsync(int trainerId, DateTime start, DateTime end, CancellationToken ct)
    {
        start = start.ToUniversalTime();
        end = end.ToUniversalTime();

        return await _context.IndividualTrainings
            .Where(t =>
                t.TrainerId == trainerId &&
                !t.IsCancelled &&
                !t.Removed &&
                start < t.EndTime &&
                end > t.StartTime)
            .AnyAsync(ct);
    }

    public async Task<IEnumerable<IndividualTraining>> GetAllIndividualTrainingsWithDetailsAsync() =>
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
            .Where(it => !it.Removed)
            .AsQueryable();

        if (filters.StartDate.HasValue)
        {
            query = query.Where(it => it.StartTime >= filters.StartDate.Value);
        }

        if (filters.EndDate.HasValue)
        {
            query = query.Where(it => it.StartTime <= filters.EndDate.Value);
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