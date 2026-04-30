namespace TaskTracker.Api.Common;

public class OperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ErrorType ErrorType { get; set; } = ErrorType.None;

    public static OperationResult Ok(string message = "")
    {
        return new OperationResult
        {
            Success = true,
            Message = message,
            ErrorType = ErrorType.None
        };
    }

    public static OperationResult Fail(string message, ErrorType errorType)
    {
        return new OperationResult
        {
            Success = false,
            Message = message,
            ErrorType =  errorType
        };
    }
}

public class OperationResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ErrorType ErrorType { get; set; } = ErrorType.None;
    public T? Data { get; set; }

    public static OperationResult<T> Ok(T data, string message = "")
    {
        return new OperationResult<T>
        {
            Success = true,
            Message = message,
            ErrorType = ErrorType.None,
            Data = data
        };
    }

    public static OperationResult<T> Fail(string message, ErrorType errorType)
    {
        return new OperationResult<T>
        {
            Success = false,
            Message = message,
            ErrorType = errorType,
            Data = default
        };
    }
}