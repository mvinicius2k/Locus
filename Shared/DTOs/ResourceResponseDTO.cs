namespace Shared.Models;

public record ResourceResponseDTO
{


    public int Id { get; init; }
    public string Path { get; init; }
    public string OwnerId { get; init; }
    public DateTime UploadedAt { get; init; }
    public string? AltText { get; init; }

    public UserResponseDTO? Owner { get; init; }
}
