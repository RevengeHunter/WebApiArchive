namespace Bussines_Library.Api.Contracts.Authors
{
    public sealed record CreateAuthorRequest(string Name, string FathersSurname, string MothersSurname, string? Nationality);
    public sealed record UpdateAuthorRequest(Guid Id, string Name, string FathersSurname, string MothersSurname, string? Nationality);
    public sealed record ParemetersAllAuthorRequest(int PageNumber = 1, int PageSize = 20, string? Search = null, string? Sort = null);
    public sealed record ParemetersByIdAuthorRequest(Guid Id);
    public sealed record DeactiveAuthorRequest(Guid Id);
}
