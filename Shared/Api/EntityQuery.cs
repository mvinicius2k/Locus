using System.Runtime.Serialization;

namespace Shared.Api;

public struct EntityQuery : IEntityQuery
{
    public int Page;
    public int PageSize;
    public string? Filter;
    public string? OrderBy;
    public string[] Expand;
    public string? Select;

    int IEntityQuery.Page => Page;

    int IEntityQuery.PageSize => PageSize;

    string? IEntityQuery.Filter => Filter;

    string? IEntityQuery.OrderBy => OrderBy;

    string[] IEntityQuery.Expand => Expand;

    string? IEntityQuery.Select => Select;

    
}
