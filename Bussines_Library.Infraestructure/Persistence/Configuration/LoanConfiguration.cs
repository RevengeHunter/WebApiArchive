using Bussines_Library.Domain.Constants;
using Bussines_Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bussines_Library.Infraestructure.Persistence.Configuration
{
    public sealed class LoanConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("Loans");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.BookId)
                .IsRequired();

            builder.Property(a => a.ApplicantName)
                .IsRequired()
                .HasMaxLength(LoanConstants.APPLICANT_NAME_MAX_LENGTH);

            builder.Property(a => a.LoanDate)
                .IsRequired();

            builder.Property(a => a.ExpectedReturnDate)
                .IsRequired();

            builder.Property(a => a.ActualReturnDate);

            builder.Property(a => a.Status)
                .IsRequired();

            // Relacion de 1 a varios con la entidad de books
            builder.HasOne(a => a.Book)
                .WithMany()
                .HasForeignKey(a => a.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
