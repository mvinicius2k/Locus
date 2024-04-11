using Api.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Api;
using System.Linq.Dynamic.Core;

namespace Api;

public static class QueryExtensions
{
    // [Obsolete]
    // public static IQueryable WithQuery<TEntity, TId>(this IQueryable<TEntity> results, IEntityQuery entityQuery) where TEntity : class, IEntity<TId>
    // {
    //     if (entityQuery.Filter != null)
    //         results = results.Where(entityQuery.Filter);
    //     if (entityQuery.OrderBy != null)
    //         results = results.OrderBy(entityQuery.OrderBy);
    //     results = results.Skip(entityQuery.PageSize * Math.Clamp(entityQuery.Page - 1, 0, int.MaxValue))
    //         .Take(entityQuery.PageSize);

    //     for (int i = 0; i < entityQuery.Expand.Length; i++)
    //         results = results.Include(entityQuery.Expand[i]);

    //     IQueryable final = entityQuery.Select != null
    //         ? results.Select(entityQuery.Select)
    //         : results;

    //     return final;
    // }

    public static IQueryable Query<TEntity>(this IEntityQuery entityQuery, ref IQueryable<TEntity> results) where TEntity : class
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
}
