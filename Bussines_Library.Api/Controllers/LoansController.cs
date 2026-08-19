using Asp.Versioning;
using Bussines_Library.Api.Contracts.Loans;
using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Features.Loans.Commands.Create;
using Bussines_Library.Application.Features.Loans.Commands.Return;
using Bussines_Library.Application.Features.Loans.DTOs;
using Bussines_Library.Application.Features.Loans.Queries.GetLoanById;
using Bussines_Library.Application.Features.Loans.Queries.GetLoans;
using Bussines_Library.Application.Features.Loans.Services;
using Bussines_Library.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bussines_Library.Api.Controllers
{
    [ApiVersion(1.0)]
    public class LoansController : BaseApiController
    {
        private readonly ILoanService _loanService;

        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        [Authorize(Policy = Polices.LoansRead)]
        [Produces(Produces.Json)]
        [ProducesResponseType(typeof(PagedResponse<LoanDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResponse<LoanDTO>>> GetLoansAsync([FromQuery] ParametersAllLoanRequest request, CancellationToken cancellationToken)
        {
            var query = new GetLoansQuery(request.PageNumber, request.PageSize, request.Search, request.Sort);
            var result = await _loanService.GetLoansAsync(query, cancellationToken);
            return await FromResultAsync(result);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Polices.LoansRead)]
        [Produces(Produces.Json)]
        [ProducesResponseType(typeof(LoanDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LoanDTO>> GetLoanByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetLoanByIdQuery(id);
            var result = await _loanService.GetLoanByIdAsync(query, cancellationToken);
            return await FromResultAsync(result);
        }

        [HttpPost]
        [Consumes(Consumes.Json)]
        [Authorize(Policy = Polices.LoansWrite)]
        [Produces(Produces.Json)]
        [ProducesResponseType(typeof(LoanDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoanDTO>> CreateLoanAsync(CreateLoanRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateLoanCommand(request.BookId, request.ApplicantName, request.ExpectedReturnDate);
            var result = await _loanService.CreateLoanAsync(command, cancellationToken);
            return await FromResultAsync(result);
        }

        [HttpPut]
        [Authorize(Policy = Polices.LoansReturn)]
        [Consumes(Consumes.Json)]
        [Produces(Produces.Json)]
        [ProducesResponseType(typeof(LoanDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<LoanDTO>> ReturnAuthorAsync(ReturnLoanRequest request, CancellationToken cancellationToken)
        {
            var command = new ReturnLoanCommand(request.Id);
            var result = await _loanService.ReturnLoanAsync(command, cancellationToken);
            return await FromResultAsync(result);
        }
    }
}
