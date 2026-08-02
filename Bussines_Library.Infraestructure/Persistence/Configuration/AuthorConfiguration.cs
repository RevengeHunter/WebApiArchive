using Bussines_Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bussines_Library.Infraestructure.Persistence.Configuration
{
    public sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Authors");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(Domain.Constants.AuthorConstants.NAME_MAX_LENGTH);

            builder.Property(a => a.FathersSurname)
                .IsRequired()
                .HasMaxLength(Domain.Constants.AuthorConstants.FATHERS_SURNAME_MAX_LENGTH);

            builder.Property(a => a.MothersSurname)
                .IsRequired()
                .HasMaxLength(Domain.Constants.AuthorConstants.MOTHERS_SURNAME_MAX_LENGTH);

            builder.Property(a => a.Nationality)
                .HasMaxLength(Domain.Constants.AuthorConstants.NATIONALITY_MAX_LENGTH);

            builder.Property(a => a.IsActive)
                .IsRequired();

            // Relacion de 1 a muchos con la entidad Book
            builder.HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
