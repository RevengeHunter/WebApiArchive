using Bussines_Library.Application.Common.Pagination;
using Bussines_Library.Application.Features.Books.Queries.GetBooks;
using Bussines_Library.Domain.Constants;
using FluentValidation;

namespace Bussines_Library.Application.Features.Authors.Validators
{
    public sealed class GetBooksQueryValidator : AbstractValidator<GetBooksQuery>
    {
        private static readonly string[] SortableFields = { "Name" };

        public GetBooksQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage(PaginatorConstants.PAGE_NUMBER_MUST_BE_GREATER_THAN_0);
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, PaginationRequest.MaxPageSize);
            RuleFor(x => x.Sort)
                .Must(BeValidSortValue)
                .When(x => !string.IsNullOrWhiteSpace(x.Sort))
                .WithMessage($"Sort must be one of the following values: {string.Join(", ", SortableFields)}.");
        }

        private static bool BeValidSortValue(string? sort)
        {
            var parts = sort?.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts is not { Length: 1 or 2 }) return false;
            if (!SortableFields.Contains(parts[0], StringComparer.OrdinalIgnoreCase)) return false;
            return parts.Length == 1 || parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase) || parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
        }
    }
}
