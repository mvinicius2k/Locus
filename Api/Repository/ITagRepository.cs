using Api.Models;

namespace Api;

public interface ITagRepository : IRepository<Tag, int>{
    public ValueTask<Tag?> GetByName(string name);
    public ValueTask<bool> DeleteByName(string name);
    public ValueTask<bool> Rename(string old, string newname);
}