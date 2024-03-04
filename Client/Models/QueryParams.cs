

namespace Client.Models;

public interface IQueryParams
{
    public int Page { get; }
    public int PageSize { get; }
    public string? OrderBy { get; }
    public (string fieldName, string value)? StringSearch { get; }
}
