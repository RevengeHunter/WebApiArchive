namespace Bussines_Library.Application.Common.Pagination
{
    public sealed record PagedResponse<T>(IEnumerable<T> Items, int PageNumber, int PageSize, int TotalCount, int TotalPages, bool HasPreviousPage, bool HasNextPage)
    {
        public static PagedResponse<T> Create(IReadOnlyCollection<T> items, int pageNumber, int pageSize, int totalCount)
        {
            var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
            var hasPreviousPage = pageNumber > 1;
            var hasNextPage = pageNumber < totalPages;

            return new PagedResponse<T>(items, pageNumber, pageSize, totalCount, totalPages, hasPreviousPage, hasNextPage);
        }
    }
}
