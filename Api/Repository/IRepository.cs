using Api.Models;
using Shared.Api;
namespace Api;

public interface IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
{
    public ValueTask<TEntity?> GetById(TId id);
    public IQueryable GetWithQuery(IEntityQuery entityQuery);
    public Task Add(TEntity entity);
    public Task<bool> TryUpdate(TId id, TEntity entity);
    public Task<bool> TryDelete(TId id);
}
