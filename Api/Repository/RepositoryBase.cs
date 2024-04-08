using Api.Database;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Api;
using System.Linq.Dynamic.Core;
using System.Runtime.CompilerServices;
namespace Api;

public class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class, IEntity<TId>
{
    protected readonly Context _context;
    protected readonly IDescribes _describes;

    public RepositoryBase(Context context, IDescribes describes)
    {
        _context = context;
        _describes = describes;
    }

    public async Task<bool> TryDelete(TId id)
    {
        var entity = _context.Set<TEntity>().Find(IdAsArray(id));
        if (entity == null)
            return false;
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public ValueTask<TEntity?> GetById(TId id)
    {
        return _context.Set<TEntity>().FindAsync(IdAsArray(id));
    }

    public IQueryable GetWithQuery(IEntityQuery entityQuery)
    {
        var results = _context.Set<TEntity>().AsQueryable();
        return Query(entityQuery, ref results);

    }


    public virtual async Task<bool> TryUpdate(TId id, TEntity newEntity)
    {
        var entity = _context.Set<TEntity>().Find(IdAsArray(id));
        if (entity == null)
            return false;
        _context.Set<TEntity>().Entry(entity).State = EntityState.Detached;
        newEntity.SetId(id);

        _context.Set<TEntity>().Update(newEntity);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Converte um dado unitário ou tupla em um array de <see langword="object"></see>.
    /// Útil para usar em conjunto com o Find do EF Core genericamente
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static object[] IdAsArray(TId key)
    {
        if (key is ITuple)
        {
            var tuple = (ITuple)key;
            var result = new object[tuple.Length];
            for (int i = 0; i < tuple.Length; i++)
            {
                result[i] = tuple[i];
            }
            return result;
        }
        else
        {
            return new object[] { key };
        }
    }
    public static IQueryable Query(IEntityQuery entityQuery, ref IQueryable<TEntity> results)
    {
        if (entityQuery.Filter != null)
            results = results.Where(entityQuery.Filter);
        if (entityQuery.OrderBy != null)
            results = results.OrderBy(entityQuery.OrderBy);
        results = results.Skip(entityQuery.PageSize * Math.Clamp(entityQuery.Page - 1, 0, int.MaxValue))
            .Take(entityQuery.PageSize);

        if (entityQuery.Expand != null)
            for (int i = 0; i < entityQuery.Expand.Length; i++)
                results = results.Include(entityQuery.Expand[i]);

        var final = entityQuery.Select != null
            ? results.Select(entityQuery.Select)
            : results;

        return final.AsQueryable();
    }

    public Task Add(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
        return _context.SaveChangesAsync();
    }


}
