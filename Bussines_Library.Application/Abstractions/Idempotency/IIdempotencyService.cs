namespace Bussines_Library.Application.Abstractions.Idempotency
{
    public interface IIdempotencyService
    {
        Task<bool> HasProcessedAsync(string key, CancellationToken cancellation = default);
        Task MarkProcessedAsync(string key, CancellationToken cancellation = default);
    }
}
