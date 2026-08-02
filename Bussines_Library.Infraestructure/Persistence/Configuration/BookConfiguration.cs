using Bussines_Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bussines_Library.Infraestructure.Persistence.Configuration
{
    public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(Book.NameMaxLength);
            builder.Property(b => b.Description)
                .IsRequired()
                .HasMaxLength(Book.DescriptionMaxLength);
            builder.Property(b => b.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(b => b.UpdatedBy)
                .IsRequired()
                .HasMaxLength(100);
            builder.HasIndex(b => b.Title).IsUnique();
            builder.HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
