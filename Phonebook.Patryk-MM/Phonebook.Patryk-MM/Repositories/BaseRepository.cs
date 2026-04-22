using Microsoft.EntityFrameworkCore;
using Phonebook.Patryk_MM.Models;
using System.Linq.Expressions;

namespace Phonebook.Patryk_MM.Repositories;
public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity {
    private readonly PhonebookDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(PhonebookDbContext dbContext) {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task AddAsync(T entity) {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity) {
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditAsync(T entity) {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes) {
    try 
    {
        IQueryable<T> query = _dbSet;

        foreach (var include in includes) {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }
    catch (Exception ex) 
    {
        Spectre.Console.AnsiConsole.WriteException(ex);
        
        Console.WriteLine("Press ANY key to exit...");
        Console.ReadLine(); 
        
        throw;
    }
}

    public async Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate) {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id) {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
