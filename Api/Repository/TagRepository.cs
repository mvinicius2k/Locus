using Api.Database;
using Api.Models;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Api;

namespace Api;

public class TagRepository : ITagRepository
{
    private readonly Context _context;

    public TagRepository(Context context)
    {
        _context = context;
    }

    public async ValueTask Add(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();  
    }

    public async ValueTask DeleteByName(string name)
    {
        var entity = await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);    

        _context.Tags.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async ValueTask<bool> ExistsByName(string name)
        => await _context.Tags.AnyAsync(t => t.Name == name);

    public async ValueTask<Tag?> GetByName(string name)
        => await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);

    public IQueryable GetWithQuery(IEntityQuery entityQuery)
    {
        var results = _context.Tags.AsQueryable();
        return entityQuery.Query(ref results);
    }

    public async ValueTask UpdateByName(Tag tag, string name)
    {
        var entity = await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);    
        tag.SetId(entity.Id);
        
        _context.Tags.Update(entity);
        await _context.SaveChangesAsync();
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
