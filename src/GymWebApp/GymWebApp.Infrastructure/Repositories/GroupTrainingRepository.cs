using GymWebApp.Application.DTOs.GroupTraining;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymWebApp.Infrastructure.Repositories;

public class GroupTrainingRepository(ApplicationDbContext context) : Repository<GroupTraining>(context), IGroupTrainingRepository
{
    public async Task<bool> ExistsOverlappingAsync(int trainerId, DateTime start, DateTime end, CancellationToken ct)
    {
        start = start.ToUniversalTime();
        end = end.ToUniversalTime();

        return await _context.GroupTrainings
            .Where(t =>
                t.TrainerId == trainerId &&
                !t.IsCancelled &&
                start < t.EndTime &&
                end > t.StartTime)
            .AnyAsync(ct);
    }

    public async Task<bool> ExistsOverlappingExcludingAsync(int trainerId, DateTime start, DateTime end, int excludeId, CancellationToken ct)
    {
        start = start.ToUniversalTime();
        end = end.ToUniversalTime();

        return await _context.GroupTrainings
            .Where(t =>
                t.TrainerId == trainerId &&
                t.Id != excludeId &&
                !t.IsCancelled &&
                start < t.EndTime &&
                end > t.StartTime)
            .AnyAsync(ct);
    }

    public async Task<IEnumerable<GroupTraining>> GetAllGroupTrainingsWithDetailsAsync() =>
        await _context.GroupTrainings
            .Include(gt => gt.Trainer)
            .Include(gt => gt.Participations)
            .Include(gt => gt.TrainingType)
            .ToListAsync();

    public async Task<IEnumerable<GroupTraining>> GetFilteredGroupTrainingsAsync(GroupTrainingFiltersDto filters)
    {
        var query = _context.GroupTrainings
            .Include(gt => gt.Trainer)
            .Include(gt => gt.TrainingType)
            .Where(gt => !gt.Removed)
            .AsQueryable();

        if (filters.StartDate.HasValue)
        {
            query = query.Where(gt => gt.StartTime >= filters.StartDate.Value);
        }

        if (filters.EndDate.HasValue)
        {
            query = query.Where(gt => gt.StartTime <= filters.EndDate.Value);
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
