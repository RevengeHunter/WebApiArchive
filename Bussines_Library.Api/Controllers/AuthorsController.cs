using Asp.Versioning;
using Bussines_Library.Application.Features.Authors.Services;
using Bussines_Library.Application.Features.Books.Services;
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
    }
}
