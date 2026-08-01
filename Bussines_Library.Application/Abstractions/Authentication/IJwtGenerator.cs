using Bussines_Library.Application.Common.TokenGenerator;

namespace Bussines_Library.Application.Abstractions.Authentication
{
    public interface IJwtGenerator
    {
        JwtTokenResult GenerateToken(Guid userId, string username, string role);
    }
}
