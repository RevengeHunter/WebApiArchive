namespace Bussines_Library.Application.Common.Pagination
{
    public sealed record PaginationRequest(int PageNumber = 1, int PageSize = 20)
    {
        public const int MaxPageSize = 100;
        public int SafePageNumber => PageNumber < 1 ? 1 : PageNumber;
        public int SafePageSize => PageSize switch //Switch expression to ensure PageSize is within valid range
        {
            < 1 => 20,
            > MaxPageSize => MaxPageSize,
            _ => PageSize
        };
    }
}
