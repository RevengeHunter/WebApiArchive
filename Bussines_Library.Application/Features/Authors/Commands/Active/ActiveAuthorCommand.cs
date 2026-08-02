namespace Bussines_Library.Application.Features.Authors.Commands.Active
{
    public sealed record ActiveAuthorCommand(
        Guid Id,
        bool IsActive
    );
}
