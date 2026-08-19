using Bussines_Library.Domain.Common;
using Bussines_Library.Domain.Constants;
using Bussines_Library.Domain.Enums;
using Bussines_Library.Domain.Exceptions;

namespace Bussines_Library.Domain.Entities
{
    public sealed class Loan : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public Book Book { get; set; } = default!;
        public string ApplicantName { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }
        public LoanStatus Status { get; set; }        

        #region Constructors

        // Constructor privado para Entity Framework
        private Loan()
        {
            ApplicantName = string.Empty;
        }

        // Constructor privado para crear una nueva instancia de Loan
        private Loan(Guid id, Guid bookId, string applicantName,
            DateTime expectedReturnDate, DateTime? actualReturnDate)
        {
            Id = id;
            SetBook(bookId);
            SetApplicantName(applicantName);
            SetLoanDates(expectedReturnDate);
            ActualReturnDate = actualReturnDate;
            Status = LoanStatus.Prestado;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Crea una nueva instancia de Loan con los datos proporcionados.
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="applicantName"></param>
        /// <param name="expectedReturnDate"></param>
        /// <returns>Una nueva instancia de Loan</returns>
        public static Loan Create(Guid bookId, string applicantName,
            DateTime expectedReturnDate)
        {
            return new Loan(Guid.NewGuid(), bookId, applicantName,
                expectedReturnDate, null);
        }

        /// <summary>
        /// Marca el préstamo como devuelto y establece la fecha de devolución real.
        /// </summary>
        public void MarkAsReturned()
        {
            ActualReturnDate = DateTime.UtcNow;
            Status = LoanStatus.Devuelto;
        }

        #endregion

        #region Private Methods

        private void SetBook(Guid bookId)
        {
            if(bookId == Guid.Empty)
            {
                throw new DomainException(LoanConstants.BOOK_ID_NOT_EMPTY,
                    LoanConstants.BOOK_ID_REQUIRED);
            }
            BookId = bookId;
        }

        private void SetApplicantName(string applicantName)
        {
            if (string.IsNullOrWhiteSpace(applicantName))
            {
                throw new DomainException(LoanConstants.APPLICANT_NAME_NOT_EMPTY,
                    LoanConstants.APPLICANT_NAME_REQUIRED);
            }
            var trimmedName = applicantName.Trim();
            if (trimmedName.Length > LoanConstants.APPLICANT_NAME_MAX_LENGTH)
            {
                throw new DomainException(LoanConstants.APPLICANT_NAME_TOO_LONG,
                    LoanConstants.APPLICANT_NAME_TOO_LONG_MESSAGE);
            }
            ApplicantName = trimmedName;
        }

        private void SetLoanDates(DateTime expectedReturnDate)
        {
            if (expectedReturnDate == default)
            {
                throw new DomainException(LoanConstants.EXPECTED_RETURN_DATE_NOT_EMPTY,
                    LoanConstants.EXPECTED_RETURN_DATE_REQUIRED);
            }

            if (expectedReturnDate < LoanDate)
            {
                throw new DomainException(
                    LoanConstants.EXPECTED_RETURN_DATE_EARLIER_THAN_LOAN_DATE,
                    LoanConstants.INVALID_DATE);
            }

            LoanDate = DateTime.UtcNow;
            ExpectedReturnDate = expectedReturnDate;
        }

        #endregion
    }
}
