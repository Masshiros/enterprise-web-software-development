using System.Linq.Expressions;

namespace Server.Application.Common.Interfaces.Persistence;

public interface IRepository<T, Key> where T : class
{
    Task<T> GetByIdAsync(Key id);

    Task<IEnumerable<T>> GetAllAsync();
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);

    void Remove(T entity);

    void RemoveRange(IEnumerable<T> entities);
}