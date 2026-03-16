namespace LHA.Ddd.Application;

/// <summary>
/// Standard API response envelope.
/// </summary>
/// <typeparam name="T">The data type.</typeparam>
public sealed class ApiResponse<T>
{
    public int StatusCode { get; init; }
    public ResponseResult<T> Result { get; init; } = new();

    /// <summary>Creates a successful response with the given data.</summary>
    public static ApiResponse<T> Ok(T data, int statusCode = 200) => new()
    {
        StatusCode = statusCode,
        Result = new ResponseResult<T> { Success = true, Data = data }
    };

    /// <summary>Creates a failed response with error details.</summary>
    public static ApiResponse<T> Fail(int statusCode, params ErrorDetailDto[] errors) => new()
    {
        StatusCode = statusCode,
        Result = new ResponseResult<T> { Success = false, Errors = [.. errors] }
    };

    /// <summary>Creates a failed response from a single message.</summary>
    public static ApiResponse<T> Fail(int statusCode, string code, string message) => new()
    {
        StatusCode = statusCode,
        Result = new ResponseResult<T>
        {
            Success = false,
            Errors = [new ErrorDetailDto { Code = code, Message = message }]
        }
    };
}

/// <summary>
/// Contains the success flag, data, and error list.
/// </summary>
public sealed class ResponseResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public List<ErrorDetailDto> Errors { get; init; } = [];
}

/// <summary>
/// Detailed error information.
/// </summary>
public sealed class ErrorDetailDto
{
    /// <summary>Machine-readable error code (e.g., "TENANT_NOT_FOUND").</summary>
    public required string Code { get; init; }

    /// <summary>Human-readable error message.</summary>
    public required string Message { get; init; }

    /// <summary>Optional field name that caused the error.</summary>
    public string? Target { get; init; }
}
