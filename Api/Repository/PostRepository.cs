using Api.Database;
using Api.Models;
using Shared;

namespace Api;

public class PostRepository : RepositoryBase<Post, int>, IPostRepository 
{
    public PostRepository(Context context, IDescribes describes) : base(context, describes)
    {

    }

    
}
