using Bussines_Library.Application.Features.Loans.Commands.Create;
using Bussines_Library.Domain.Constants;
using FluentValidation;

namespace Bussines_Library.Application.Features.Loans.Validators
{
    public sealed class CreateLoanCommandValidator : AbstractValidator<CreateLoanCommand>
    {
        public CreateLoanCommandValidator()
        {
            RuleFor(x => x.BookId)
                .NotEmpty()
                .WithMessage(LoanConstants.BOOK_ID_NOT_EMPTY);

            RuleFor(x => x.ApplicantName)
                .NotEmpty()
                .WithMessage(LoanConstants.APPLICANT_NAME_REQUIRED)
                .MaximumLength(LoanConstants.APPLICANT_NAME_MAX_LENGTH)
                .WithMessage(LoanConstants.APPLICANT_NAME_TOO_LONG_MESSAGE);

            RuleFor(x => x.ExpectedReturnDate)
                .NotEmpty()
                .WithMessage(LoanConstants.EXPECTED_RETURN_DATE_NOT_EMPTY);
        }
    }
}
