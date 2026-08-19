using Bussines_Library.Application.Abstractions.Data;
using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Common.Results;
using Bussines_Library.Application.Features.Loans.Commands.Create;
using Bussines_Library.Application.Features.Loans.Commands.Return;
using Bussines_Library.Application.Features.Loans.DTOs;
using Bussines_Library.Application.Features.Loans.Mappings;
using Bussines_Library.Application.Features.Loans.Parameters;
using Bussines_Library.Application.Features.Loans.Queries.GetLoanById;
using Bussines_Library.Application.Features.Loans.Queries.GetLoans;
using Bussines_Library.Application.Features.Loans.Repositories;
using Bussines_Library.Domain.Constants;
using Bussines_Library.Domain.Entities;
using FluentValidation;

namespace Bussines_Library.Application.Features.Loans.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateLoanCommand> _createLoanValidator;
        private readonly IValidator<GetLoansQuery> _getLoansValidator;

        public LoanService(ILoanRepository loanRepository,
            IUnitOfWork unitOfWork,
            IValidator<CreateLoanCommand> createLoanValidator,
            IValidator<GetLoansQuery> getLoansValidator)
        {
            _loanRepository = loanRepository;
            _unitOfWork = unitOfWork;
            _createLoanValidator = createLoanValidator;
            _getLoansValidator = getLoansValidator;
        }

        public async Task<Result<LoanDTO>> CreateLoanAsync(CreateLoanCommand createLoanCommand, CancellationToken cancellationToken = default)
        {
            var validation = await _createLoanValidator.ValidateAsync(createLoanCommand, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary( g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return Result<LoanDTO>.ValidationFailure(errors);
            }

            if (await _loanRepository.HasActiveLoanAsync(createLoanCommand.BookId, cancellationToken))
            {
                return Result<LoanDTO>.Failure(new Error(LoanConstants.BOOK_ALREADY_LOANED, LoanConstants.BOOK_ALREADY_LOANED_MESSAGE,ErrorType.Conflict));
            }

            var loan = Loan.Create(
                createLoanCommand.BookId,
                createLoanCommand.ApplicantName,
                createLoanCommand.ExpectedReturnDate
            );

            await _loanRepository.AddAsync(loan, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<LoanDTO>.Success(loan.ToDTO());
        }

        public async Task<Result<LoanDTO>> GetLoanByIdAsync(GetLoanByIdQuery getLoanByIdQuery, CancellationToken cancellationToken = default)
        {
            var loan = await _loanRepository.GetByIdAsync(getLoanByIdQuery.Id, cancellationToken);
            if(loan is null)
            {
                return Result<LoanDTO>.Failure(new Error(LoanConstants.LOAN_NOT_FOUND, LoanConstants.LOAN_NOT_FOUND_MESSAGE, ErrorType.NotFound));
            }

            return Result<LoanDTO>.Success(loan.ToDTO());
        }

        public async Task<Result<PagedResponse<LoanDTO>>> GetLoansAsync(GetLoansQuery getLoansQuery, CancellationToken cancellation = default)
        {
            var validation = await _getLoansValidator.ValidateAsync(getLoansQuery, cancellation);
            if (!validation.IsValid)
            {
                return Result<PagedResponse<LoanDTO>>.ValidationFailure(validation.ToDictionary().AsReadOnly());
            }

            var page = await _loanRepository.GetAllAsync(new LoanQueryParameters(new PaginationRequest(getLoansQuery.PageNumber, getLoansQuery.PageSize), getLoansQuery.Search, getLoansQuery.Sort), cancellation);

            var loans = page.Items.Select(a => a.ToDTO()).ToList();

            return Result<PagedResponse<LoanDTO>>.Success(PagedResponse<LoanDTO>.Create(loans, page.PageNumber, page.PageSize, page.TotalCount));
        }

        public async Task<Result<LoanDTO>> ReturnLoanAsync(ReturnLoanCommand returnLoanCommand, CancellationToken cancellation = default)
        {
            var loan = await _loanRepository.GetByIdAsync(returnLoanCommand.Id);
            
            if (loan is null)
            {
                return Result<LoanDTO>.Failure(new Error(LoanConstants.LOAN_NOT_FOUND, LoanConstants.LOAN_NOT_FOUND_MESSAGE, ErrorType.NotFound));
            }

            loan.MarkAsReturned();
            await _unitOfWork.SaveChangesAsync(cancellation);
            return Result<LoanDTO>.Success(loan.ToDTO());
        }
    }
}
