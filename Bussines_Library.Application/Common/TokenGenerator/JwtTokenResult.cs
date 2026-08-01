namespace Bussines_Library.Application.Common.TokenGenerator
{
    public sealed record JwtTokenResult(string AccessToken, DateTime ExpiresAt);
}
