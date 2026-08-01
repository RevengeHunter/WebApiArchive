using Bussines_Library.Application.Common.TokenGenerator;

namespace Bussines_Library.Application.Features.Authentication.Services
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> LoginAsync(string username, string password, CancellationToken cancellationToken);
    }
}
