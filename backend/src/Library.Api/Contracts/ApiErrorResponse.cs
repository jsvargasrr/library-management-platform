namespace Library.Api.Contracts;

public sealed class ApiErrorResponse
{
    public required bool Success { get; init; }
    public required string Message { get; init; }
    public required IReadOnlyCollection<string> Errors { get; init; }
    public required string TraceId { get; init; }
}

