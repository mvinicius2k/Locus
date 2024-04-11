using Api.Database;
using Api.Models;
using Shared;

namespace Api;

public class ImageRepository : RepositoryBase<Image, int>, IImageRepository
{
    public ImageRepository(Context context, IDescribes describes) : base(context, describes)
    {
    }
}
