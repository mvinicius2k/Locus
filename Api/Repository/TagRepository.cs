using Api.Database;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Api;

public class TagRepository : RepositoryBase<Tag, int>, ITagRepository
{
    public TagRepository(Context context, IDescribes describes) : base(context, describes)
    {
    }

    public async ValueTask<bool> DeleteByName(string name)
    {
        var entity = await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);    
        if (entity == null)
            return false;
        _context.Tags.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async ValueTask<Tag?> GetByName(string name)
        => await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);

    public async ValueTask<bool> Rename(string old, string newname)
    {
        var entity = await _context.Tags.FirstOrDefaultAsync(t => t.Name == old);    
        if (entity == null)
            return false;
        entity.Name = newname;
        _context.Tags.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }




    // public override async Task<bool> TryUpdate(string id, Tag newEntity)
    // {

    //     var entity = _context.Tags.Find(IdAsArray(id));
    //     if (entity == null)
    //         return false;
    //     _context.Tags.Entry(entity).State = EntityState.Detached;
    //     entity.Name = newEntity.Name;

    //     _context.Tags.Update(newEntity);
    //     await _context.SaveChangesAsync();
    //     return true;
    // }

}
