using Api.Models;
using Shared.Api;
namespace Api;

public interface IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
{
    public ValueTask<TEntity?> GetById(TId id);
    public IQueryable GetWithQuery(IEntityQuery entityQuery);
    public ValueTask Add(TEntity entity);
    public ValueTask Update(TEntity entity, TId id);
    public ValueTask Delete(TId index);
    public ValueTask<bool> Exists(TId id);
}
