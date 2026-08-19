using Bussines_Library.Application.Abstractions.Clock;
using Bussines_Library.Application.Abstractions.CurrentUser;
using Bussines_Library.Application.Abstractions.Data;
using Bussines_Library.Domain.Common;
using Bussines_Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussines_Library.Infraestructure.Persistence
{
    public sealed class ApplicationDbContext: DbContext, IUnitOfWork
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly ICurrentUserService currentUserService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeProvider dateTimeProvider, ICurrentUserService currentUserService)
            : base(options)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.currentUserService = currentUserService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAudit() 
        {
            var now = dateTimeProvider.UtcNow;
            var currentUserId = currentUserService.UserId;

            foreach ( var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.State == EntityState.Added) entry.Entity.MarkCreated(now, currentUserId);

                if (entry.State == EntityState.Modified) entry.Entity.MarkUpdated(now, currentUserId); 

            }
        }

        // Define your DbSets for entities here
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Loan> Loans => Set<Loan>();
    }
}
