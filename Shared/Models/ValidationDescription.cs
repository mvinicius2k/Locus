namespace Shared;

public record ValidationDescription : IValidationDescription{
    public required string PropertyName {get; init;}
    public required string Message {get; init;}
}