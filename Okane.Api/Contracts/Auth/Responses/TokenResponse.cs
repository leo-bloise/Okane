namespace Okane.Api.Contracts.Auth.Responses;

public sealed record TokenResponse(string AccessToken, DateTimeOffset ExpiresAt);
