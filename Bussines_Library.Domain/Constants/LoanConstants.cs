namespace Bussines_Library.Domain.Constants
{
    public static class LoanConstants
    {
        public const int APPLICANT_NAME_MAX_LENGTH = 300;

        public const string APPLICANT_NAME_REQUIRED = "Applicant name is required.";
        public const string BOOK_ID_REQUIRED = "Book ID is required.";
        public const string EXPECTED_RETURN_DATE_REQUIRED = "Expected return date is required.";

        public const string APPLICANT_NAME_NOT_EMPTY = "Applicant name cannot be null or empty.";
        public const string BOOK_ID_NOT_EMPTY = "Book ID cannot be null or empty.";
        public const string EXPECTED_RETURN_DATE_NOT_EMPTY = "Expected return date cannot be null or empty.";

        public const string APPLICANT_NAME_TOO_LONG = "Applicant name is too long.";

        public static readonly string APPLICANT_NAME_TOO_LONG_MESSAGE = $"Applicant name cannot exceed {APPLICANT_NAME_MAX_LENGTH} characters.";

        public const string EXPECTED_RETURN_DATE_EARLIER_THAN_LOAN_DATE = "Expected return date cannot be earlier than loan date.";
        public const string INVALID_DATE = "Invalid return date.";

        public const string BOOK_ALREADY_LOANED = "Already on loan";
        public const string BOOK_ALREADY_LOANED_MESSAGE = "The book is already on loan.";

        public const string LOAN_NOT_FOUND = "Not Found";
        public const string LOAN_NOT_FOUND_MESSAGE = "The loan was not found.";
    }
}
