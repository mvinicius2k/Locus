namespace Api;

public record GoogleOAuthSettings
{
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
}