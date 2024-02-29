namespace Shared.Models
{
    public record PostResponseDTO
    {

        public const int TitleMaxLength = 64;
        public const int ContentMaxLength = 15000;

        public int Id { get; init; }
        public string Title { get; init; }
        public string Content { get; init; }
        public int FeaturedImageId { get; init; }
        public string OwnerId { get; init; }

        public UserResponseDTO Owner { get; init; }
        public ResourceResponseDTO FreaturedImage { get; init; }
        public ICollection<TagResponseDTO> Tags { get; init; }

    }

}
