namespace Shared.Models
{
    public record PostTagResponseDTO
    {
        public string TagName { get; init; }
        public int PostId { get; init; }

        public TagResponseDTO Tag { get; init; }
        public PostResponseDTO Post { get; init; }
    }
}
