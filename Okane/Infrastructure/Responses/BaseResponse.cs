namespace Okane.Infrastructure.Responses;

public class BaseResponse
{
    public string Message { get; set; } = string.Empty;

    public int Status { get; set; }

    public long Timestamp { get; set; }
}
