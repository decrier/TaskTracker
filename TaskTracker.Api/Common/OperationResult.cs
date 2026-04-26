namespace TaskTracker.Api.Common;

public class OperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static OperationResult Ok(string message = "")
    {
        return new OperationResult
        {
            Success = true,
            Message = message
        };
    }

    public static OperationResult Fail(string message)
    {
        return new OperationResult
        {
            Success = false,
            Message = message
        };
    }
}

public class OperationResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static OperationResult<T> Ok(T data, string message = "")
    {
        return new OperationResult<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static OperationResult<T> Fail(string message)
    {
        return new OperationResult<T>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}