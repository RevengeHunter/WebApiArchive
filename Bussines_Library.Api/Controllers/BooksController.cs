using Asp.Versioning;
using Bussines_Library.Api.Contracts.Books;
using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Features.Books.Commads.CreateBook;
using Bussines_Library.Application.Features.Books.Commads.DeleteBook;
using Bussines_Library.Application.Features.Books.Commads.UpdateBook;
using Bussines_Library.Application.Features.Books.DTOs;
using Bussines_Library.Application.Features.Books.Queries.GetBookById;
using Bussines_Library.Application.Features.Books.Queries.GetBooks;
using Bussines_Library.Application.Features.Books.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bussines_Library.Api.Controllers
{
    //[Authorize(Roles = "Administrator")]
    [ApiVersion(1.0)]
    //[Route("Book")]
    public class BooksController : BaseApiController
    {
        private readonly IBookService bookService;

        public BooksController(IBookService bookService) => this.bookService = bookService;

        [HttpGet]
        [Authorize(Policy ="Books.Read")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(PagedResponse<BookDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<BookDTO>>> GetBooks([FromQuery(Name = "page-number")] int pageNumber = 1, [FromQuery(Name = "page-size")] int pageSize = 20,
            [FromQuery] string? search = null, [FromQuery] string? sort = null, CancellationToken cancellationToken = default)
        {

            var result = await bookService.GetBooksAsync(new GetBooksQuery(pageNumber, pageSize, search, sort), cancellationToken);

            return await FromResultAsync(result);

        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [Authorize(Policy = "Books.Read")]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDTO>> GetBookId(Guid id, CancellationToken cancellationToken)
        {
            var result = await bookService.GetBooksAsync(new GetBookByIdQuery(id), cancellationToken);

            return await FromResultAsync(result);
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Authorize(Policy = "Books.Write")]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BookDTO>> CreateBook([FromBody] CreateBookRequest request, [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey, CancellationToken cancellationToken)
        {
            var result = await bookService.CreateBookAsync(new CreateBookCommand(request.Title, request.Description), cancellationToken);

            if (result.IsSuccess && result.Value is not null)
            {
                return await CreatedResponseAsync(
                    nameof(GetBookId),
                    new { id = result.Value.Id, version = "1" },
                    result.Value);
            }

            return await FromResultAsync(result);
        }

        [HttpPut("{id:guid}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<BookDTO>> UpdateBook(Guid id, [FromBody] UpdateBookRequest request, CancellationToken cancellationToken)
        {
            var result = await bookService.UpdateBookAsync(new UpdateBookCommand(id, request.Title, request.Description), cancellationToken);
            return await FromResultAsync(result);
        }

        [HttpPatch("{id:guid}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<BookDTO>> PatchBook(Guid id, [FromBody] PatchBookRequest request, CancellationToken cancellationToken)
        {
            var result = await bookService.PatchBookAsync(new UpdateBookCommand(id, request.Title, request.Description), cancellationToken);
            return await FromResultAsync(result);
        }

        [HttpDelete("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteBook(Guid id, CancellationToken cancellationToken)
        {
            var result = await bookService.DeleteBookAsync(new DeleteBookCommand(id), cancellationToken);

            return result.IsSuccess ? await NoContentResponseAsync() : await FromResultIActionAsync(result);
        }

    }
}
