using Microsoft.EntityFrameworkCore;
using Phonebook.Models;
using System.Linq.Expressions;

namespace Phonebook.Repositories;
public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity {
    internal readonly AppDbContext DbContext;
    internal readonly DbSet<T> DbSet;

    public BaseRepository(AppDbContext dbContext, DbSet<T> dbSet) {
        DbContext = dbContext;
        DbSet = dbSet;
    }

    public async Task AddAsync(T entity) {
        await DbSet.AddAsync(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity) {
        DbSet.Remove(entity);
        await DbContext.SaveChangesAsync();
    }

    public async Task EditAsync(T entity) {
        DbSet.Update(entity);
        await DbContext.SaveChangesAsync();
    }

    public Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] include) {
        IQueryable<T> query = DbSet;

        foreach (var inc in include) {
            query = query.Include(inc);
        }
        return query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id) {
        return await DbSet.FindAsync(id);
    }
}
