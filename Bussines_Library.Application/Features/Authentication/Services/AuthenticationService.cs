using Bussines_Library.Application.Abstractions.Authentication;
using Bussines_Library.Application.Common.TokenGenerator;

namespace Bussines_Library.Application.Features.Authentication.Services
{
    public sealed class AuthenticationService : IAuthenticationService
    {

        private readonly IJwtGenerator _jwtTokenGenerator;

        public AuthenticationService(IJwtGenerator jwtGenerator)
        {
            _jwtTokenGenerator = jwtGenerator;
        }

        public Task<AuthenticationResult> LoginAsync(string username, string password, CancellationToken cancellationToken)
        {
            //Solo con fin de hacer un ejemplo academico
            const string demoUserName = "admin";
            const string demoPassword = "Admin123!";

            if (username != demoUserName || password != demoPassword)
                return Task.FromResult(new AuthenticationResult(false, null, null, null, "Usuario o contraseña incorrectos."));

            var token = _jwtTokenGenerator.GenerateToken(Guid.Parse("11111111-1111-1111-1111-111111111111"), username, "Administrator");

            return Task.FromResult(new AuthenticationResult(true, token.AccessToken, token.ExpiresAt, username, null));
        }
    }
}
