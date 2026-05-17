namespace Okane.Infrastructure.Responses;

public class ResponsesFacade
{
    public static BaseResponse CreateBaseResponse(string message, int status)
    {
        return new BaseResponse()
        {
            Message = message,
            Status = status,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        };
    }

    public static DetailedResponse CreateDetailedResponse(string message, int status, IReadOnlyDictionary<string, object?> details)
    {
        return new DetailedResponse()
        {
            Message = message,
            Status = status,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Details = details,
        };
    }

    public static BaseResponse Ok(string message, IReadOnlyDictionary<string, object?> details)
    {
        return CreateDetailedResponse(message, 200, details);
    }

    public static BaseResponse Ok(string message)
    {
        return CreateBaseResponse(message, 200);
    }

    public static BaseResponse UnprocessableEntity(string message)
    {
        return CreateBaseResponse(message, 422);
    }

    public static BaseResponse BadRequest(string message)
    {
        return CreateBaseResponse(message, 400);
    }

    public static BaseResponse NotFound(string message)
    {
        return CreateBaseResponse(message, 404);
    }

    public static BaseResponse InternalServerError(string message)
    {
        return CreateBaseResponse(message, 500);
    }

    public static DetailedResponse ValidationError(string message, IReadOnlyDictionary<string, object?> details)
    {
        return CreateDetailedResponse(message, 422, details);
    }

    public static DetailedResponse UnprocessableEntity(string message, IReadOnlyDictionary<string, object?> details)
    {
        return CreateDetailedResponse(message, 422, details);
    }

    public static DetailedResponse Created(string message, IReadOnlyDictionary<string, object?> details)
    {
        return CreateDetailedResponse(message, 201, details);
    }
}
