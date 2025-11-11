using GymWebApp.Data.Entities.Abstract;
using GymWebApp.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymWebApp.Data.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(object id) =>
        await _dbSet.FirstOrDefaultAsync(e => e.Id.Equals(id) && !e.Removed);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.Where(e => !e.Removed).ToListAsync();

    public async Task AddAsync(T entity) =>
        await _dbSet.AddAsync(entity);

    public void Update(T entity) =>
        _dbSet.Update(entity);

    public void Remove(T entity)
    {
        entity.Removed = true;
        _dbSet.Update(entity);
    }

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
