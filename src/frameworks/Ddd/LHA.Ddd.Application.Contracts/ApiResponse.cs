namespace LHA.Ddd.Application;

/// <summary>
/// Standard API response envelope.
/// </summary>
/// <typeparam name="T">The data type.</typeparam>
public sealed class ApiResponse<T>
{
    /// <summary>
    /// HTTP status code of the response (e.g., 200, 201, 400).
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// Contains the success flag, payload data, and error details.
    /// </summary>
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
    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// The response payload. <c>null</c> when <see cref="Success"/> is <c>false</c>.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Collection of error details. Empty when <see cref="Success"/> is <c>true</c>.
    /// </summary>
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
