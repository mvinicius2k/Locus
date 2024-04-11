using Api.Models;
using Shared.Api;

namespace Api;

public interface ITagRepository {
    public ValueTask<Tag?> GetByName(string name);
    public ValueTask DeleteByName(string name);
    public ValueTask UpdateByName(Tag tag, string name);
    public ValueTask<bool> ExistsByName(string name);
    public IQueryable GetWithQuery(IEntityQuery entityQuery);
    public ValueTask Add(Tag tag);
    
}