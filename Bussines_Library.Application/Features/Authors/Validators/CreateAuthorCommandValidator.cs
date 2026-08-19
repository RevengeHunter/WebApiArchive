using Bussines_Library.Application.Features.Authors.Commands.Create;
using Bussines_Library.Domain.Constants;
using FluentValidation;

namespace Bussines_Library.Application.Features.Authors.Validators
{
    public sealed class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
    {
        public CreateAuthorCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(AuthorConstants.NAME_REQUIRED)
                .MaximumLength(AuthorConstants.NAME_MAX_LENGTH).WithMessage(AuthorConstants.NAME_MAX_LENGTH_MESSAGE);

            RuleFor(x => x.FathersSurname)
                .NotEmpty().WithMessage(AuthorConstants.FATHERS_SURNAME_REQUIRED)
                .MaximumLength(AuthorConstants.FATHERS_SURNAME_MAX_LENGTH).WithMessage(AuthorConstants.FATHERS_SURNAME_MAX_LENGTH_MESSAGE);

            RuleFor(x => x.MothersSurname)
                .NotEmpty().WithMessage(AuthorConstants.MOTHERS_SURNAME_REQUIRED)
                .MaximumLength(AuthorConstants.MOTHERS_SURNAME_MAX_LENGTH).WithMessage(AuthorConstants.MOTHERS_SURNAME_MAX_LENGTH_MESSAGE);

            RuleFor(x => x.Nationality)
                .MaximumLength(AuthorConstants.NATIONALITY_MAX_LENGTH).WithMessage(AuthorConstants.NATIONALITY_MAX_LENGTH_MESSAGE);
        }
    }
}
