namespace Bussines_Library.Application.Features.Authors.Commands.Create
{
    public sealed record CreateAuthorCommand(
        string Name,
        string FathersSurname,
        string MothersSurname,
        string? Nationality
    );
}
