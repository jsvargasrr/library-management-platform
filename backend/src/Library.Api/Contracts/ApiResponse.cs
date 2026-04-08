namespace Library.Api.Contracts;

public sealed class ApiResponse<T>
{
    public required bool Success { get; init; }
    public required string Message { get; init; }
    public required T? Data { get; init; }
    public required string TraceId { get; init; }
}

