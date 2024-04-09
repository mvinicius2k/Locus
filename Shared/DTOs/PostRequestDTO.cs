namespace Shared;

public record PostRequestDTO
{
    public string Title { get; init; }

    public string Content { get; init; }
    public string Summary { get; init; }

    public int? FeaturedImageId { get; init; }

    public string OwnerId { get; init; }
}
