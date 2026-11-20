namespace Application.Common;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unexpected
}

/// <summary>Résultat d'un cas d'utilisation sans valeur de retour.</summary>
public sealed class Result
{
    public bool IsSuccess { get; }

    public string? Error { get; }

    public ErrorType ErrorType { get; }

    private Result(bool isSuccess, string? error, ErrorType errorType)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }

    public static Result Success() => new(true, null, default);

    public static Result Failure(string error, ErrorType errorType = ErrorType.Validation)
        => new(false, error, errorType);
}

/// <summary>Résultat d'un cas d'utilisation avec valeur de retour.</summary>
public sealed class Result<T>
{
    public bool IsSuccess { get; }

    public T? Value { get; }

    public string? Error { get; }

    public ErrorType ErrorType { get; }

    private Result(bool isSuccess, T? value, string? error, ErrorType errorType)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorType = errorType;
    }

    public static Result<T> Success(T value) => new(true, value, null, default);

    public static Result<T> Failure(string error, ErrorType errorType = ErrorType.Validation)
        => new(false, default, error, errorType);
}
