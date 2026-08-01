namespace Bussines_Library.Api.Contracts.Authentication
{
    public sealed record LoginResponse(string AccessToken, DateTime ExpiresAt, string TokenType, string Username);
}
