namespace Okane.Infrastructure.Responses;

public class DetailedResponse : BaseResponse
{
    public IReadOnlyDictionary<string, object>? Details { get; set; } = null;
}
