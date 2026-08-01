using Asp.Versioning;
using Bussines_Library.Api.Contracts.Authentication;
using Bussines_Library.Application.Features.Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bussines_Library.Api.Controllers
{
    [ApiVersion(1.0)]
    public class AuthenticationController : BaseApiController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authenticationService.LoginAsync(request.Username, request.Password, cancellationToken);

            if (!result.isSuccess)
            {
                return Unauthorized(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Invalid Credentials.",
                    Detail = result.ErrorMessage
                });
            }

            return Ok(new LoginResponse(result.AccessToken!, result.ExpiresAt!.Value, "Bearer", result.Username!));

        }
    }
}
