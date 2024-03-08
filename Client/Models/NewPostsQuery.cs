using Client.Models;

namespace Client;

public struct NewPostsQuery : IQueryParams
{
    public int Page {get; set;}

    public readonly int PageSize => 10;

    public readonly string? OrderBy => null;

    public readonly (string fieldName, string value)? StringSearch => null;
}
