using Api.Database;
using Api.Models;

namespace Api;

public class TagRepository : RepositoryBase<Tag, string>, ITagRepository
{
    public TagRepository(Context context, IDescribes describes) : base(context, describes)
    {
    }


}
