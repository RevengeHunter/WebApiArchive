namespace Bussines_Library.Application.Features.Authors.Commands.Update
{
    public sealed record UpdateAuthorCommand(
        Guid Id,
        string Name,
        string FathersSurname,
        string MothersSurname,
        string? Nationality
    );
}
