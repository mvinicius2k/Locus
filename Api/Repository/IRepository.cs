using Api.Models;
using Shared.Api;
namespace Api;

public interface IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
{
    public ValueTask<TEntity?> GetById(TId id);
    public IQueryable GetWithQuery(IEntityQuery entityQuery);
    public Task Add(TEntity entity);
    public Task Update(TId id, TEntity entity);
    public Task Delete(TId id);
}
