using Bussines_Library.Application.Abstractions.Authentication;
using Bussines_Library.Application.Common.TokenGenerator;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bussines_Library.Infraestructure.Authentication
{
    public sealed class JwtTokenGenerator : IJwtGenerator
    {
        private readonly JWTOptions _jwtOptions;

        public JwtTokenGenerator(IOptions<JWTOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public JwtTokenResult GenerateToken(Guid userId, string username, string role)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationInMinutes);
            var claims = new List<Claim> {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(ClaimTypes.NameIdentifier, userId.ToString()),
                new(ClaimTypes.Name, username),
                new(ClaimTypes.Role, role),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Expires = expiresAt,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var accessToken = tokenHandler.WriteToken(token);

            return new JwtTokenResult(accessToken, expiresAt);
        }
    }
}
