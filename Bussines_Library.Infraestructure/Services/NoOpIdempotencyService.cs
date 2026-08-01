using Bussines_Library.Application.Abstractions.Idempotency;

namespace Bussines_Library.Infraestructure.Services
{
    public sealed class NoOpIdempotencyService : IIdempotencyService
    {
        public Task<bool> HasProcessedAsync(string key, CancellationToken cancellation = default) => Task.FromResult(false);

        public Task MarkProcessedAsync(string key, CancellationToken cancellation = default) => Task.CompletedTask;
    }
}
