namespace Okane.Api.Contracts;

public record ApiResponse
{
    public required string Message { get; init; }

    public required int Status { get; init; }

    public long Timestamp { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();    
}

public sealed record ApiResponse<TData> : ApiResponse
{
    public TData? Details { get; init; }
}
