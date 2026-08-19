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

    }
}
