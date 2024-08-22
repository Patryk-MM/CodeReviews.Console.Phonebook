using Phonebook.Models;
using System.Linq.Expressions;

namespace Phonebook.Repositories;
public interface IBaseRepository<T> where T : BaseEntity {
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetItemAsync(Expression<Func<T, bool>> include);
    Task<List<T>> GetAsync(params Expression<Func<T, object>>[] include);
    Task AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task EditAsync(T entity);
}