namespace Bussines_Library.Application.Common.TokenGenerator
{
    public sealed record AuthenticationResult(bool isSuccess, string? AccessToken, DateTime? ExpiresAt, string? Username, string? ErrorMessage);
}
