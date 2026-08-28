namespace Okane.Api.Contracts;

public static class ApiResponseFactory
{
    public static ApiResponse<TData> Success<TData>(TData data, string message, int status = StatusCodes.Status200OK)
        => new() { Message = message, Status = status, Details = data };


    public static ApiResponse<TData> Error<TData>(string message, int status, TData data)
        => new() { Message = message, Status = status, Details = data };

    public static ApiResponse Error(string message, int status)
        => new() { Message = message, Status = status };
}
