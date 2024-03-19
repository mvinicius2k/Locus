using Api.Database;
using Api.Models;
using Shared;

namespace Api;

public class TagRepository : RepositoryBase<Tag, string>, ITagRepository
{
    public TagRepository(Context context, IDescribes describes) : base(context, describes)
    {
    }


}
