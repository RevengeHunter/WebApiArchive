using Bussines_Library.Application.Features.Books.Commads.UpdateBook;
using Bussines_Library.Domain.Entities;
using FluentValidation;

namespace Bussines_Library.Application.Features.Books.Validators
{
    public sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
    {
        public UpdateBookCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(Book.NameMaxLength).WithMessage("Title must not exceed 100 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(Book.DescriptionMaxLength).WithMessage("Description must not exceed 500 characters.");
        }
    }
}
