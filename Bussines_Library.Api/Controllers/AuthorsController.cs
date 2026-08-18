using Asp.Versioning;
using Bussines_Library.Api.Contracts.Authors;
using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Features.Authors.Commands.Active;
using Bussines_Library.Application.Features.Authors.Commands.Create;
using Bussines_Library.Application.Features.Authors.Commands.Update;
using Bussines_Library.Application.Features.Authors.DTOs;
using Bussines_Library.Application.Features.Authors.Parameters;
using Bussines_Library.Application.Features.Authors.Queries.GetAuthorById;
using Bussines_Library.Application.Features.Authors.Queries.GetAuthors;
using Bussines_Library.Application.Features.Authors.Services;
using Bussines_Library.Application.Features.Books.Services;
using Bussines_Library.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bussines_Library.Api.Controllers
{
    [ApiVersion(1.0)]
    public class AuthorsController : BaseApiController
    {
        private readonly IAuthorService authorService;

        public AuthorsController(IAuthorService authorService)
        {
            this.authorService = authorService;
        }

        [HttpGet]
        [Authorize(Policy = Polices.AuthorsRead)]
        [Produces(Produces.Json)]
        [ProducesResponseType(typeof(PagedResponse<AuthorDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<AuthorDTO>>> GetAuthorsAsync([FromQuery] ParemetersAllAuthorRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAuthorsQuery(request.PageNumber, request.PageSize, request.Search, request.Sort);
            var result = await authorService.GetAuthorsAsync(query, cancellationToken);
            return await FromResultAsync(result);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Polices.AuthorsRead)]
        [Produces(Produces.Json)]
        [ProducesResponseType(typeof(AuthorDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorDTO>> GetAuthorByIdAsync([FromQuery] ParemetersByIdAuthorRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAuthorByIdQuery(request.Id);
            var result = await authorService.GetAuthorByIdAsync(query, cancellationToken);
            return await FromResultAsync(result);
        }

        [HttpPost]
        [Consumes(Consumes.Json)]
        [Authorize(Policy = Polices.AuthorsWrite)]
        [Produces(Produces.Json)]
        [ProducesResponseType(typeof(AuthorDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthorDTO>> CreateAuthorAsync(CreateAuthorRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateAuthorCommand(request.Name, request.FathersSurname, request.MothersSurname, request.Nationality);
            var result = await authorService.CreateAuthorAsync(command, cancellationToken);
            return await FromResultAsync(result);
        }

        [HttpPut]
        [Authorize(Policy = Polices.AuthorsWrite)]
        [Consumes(Consumes.Json)]
        [Produces(Produces.Json)]
        [ProducesResponseType(typeof(AuthorDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthorDTO>> UpdateAuthorAsync(UpdateAuthorRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateAuthorCommand(request.Id, request.Name, request.FathersSurname, request.MothersSurname, request.Nationality);
            var result = await authorService.UpdateAuthorAsync(command, cancellationToken);
            return await FromResultAsync(result);
        }

        [HttpDelete]
        [Produces(Produces.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAuthorAsync([FromQuery] DeactiveAuthorRequest request, CancellationToken cancellationToken)
        {
            var command = new ActiveAuthorCommand(request.Id);
            var result = await authorService.DactiveAuthorAsync(command, cancellationToken);
            return result.IsSuccess ? await NoContentResponseAsync() : await FromResultIActionAsync(result);
        }
    }
}
