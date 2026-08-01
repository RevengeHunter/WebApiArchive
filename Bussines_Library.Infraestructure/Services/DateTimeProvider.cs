using Bussines_Library.Application.Abstractions.Clock;

namespace Bussines_Library.Infraestructure.Services
{
    public sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
