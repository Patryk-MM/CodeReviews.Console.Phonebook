using Phonebook.Models;
using System.Linq.Expressions;

namespace Phonebook.Repositories;
public interface IBaseRepository<T> where T : BaseEntity {
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] include);
    Task AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task EditAsync(T entity);
}