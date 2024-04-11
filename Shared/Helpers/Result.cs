namespace Shared;

public record Result<T>
{
    public bool IsSuccess { get; init;}

    public bool IsFailure => !IsSuccess;

    public Error Error { get; init;}
    public T Model {get; init;}

    public static Result<T> Success(T data) => new(true, Error.None, data);

    public static Result<T> Failure(Error error) => new(false, error, default);

    private Result(bool isSuccess, Error error, T data)
    {

        IsSuccess = isSuccess;
        Error = error;
        Model = data;
        
    }

    
}

public record Error(int Code, string Description)
{
    public static readonly Error None = new(-1, string.Empty);
}
