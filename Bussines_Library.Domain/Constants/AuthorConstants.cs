namespace Bussines_Library.Domain.Constants
{
    public static class AuthorConstants
    {
        public const int NAME_MAX_LENGTH = 250;
        public const int FATHERS_SURNAME_MAX_LENGTH = 250;
        public const int MOTHERS_SURNAME_MAX_LENGTH = 250;
        public const int NATIONALITY_MAX_LENGTH = 200;

        public const string NAME_REQUIRED = "Name is required.";
        public const string FATHERS_SURNAME_REQUIRED = "Fathers surname is required.";
        public const string MOTHERS_SURNAME_REQUIRED = "Mothers surname is required.";
        
        public const string NAME_NOT_EMPTY = "Name cannot be null or empty.";
        public const string FATHERS_SURNAME_NOT_EMPTY = "Fathers surname cannot be null or empty.";
        public const string MOTHERS_SURNAME_NOT_EMPTY = "Mothers surname cannot be null or empty.";

        public const string NAME_TOO_LONG = "Name is too long.";
        public const string FATHERS_SURNAME_TOO_LONG = "Fathers surname is too long.";
        public const string MOTHERS_SURNAME_TOO_LONG = "Mothers surname is too long.";
        public const string NATIONALITY_TOO_LONG = "Nationality is too long.";

        public static readonly string NAME_MAX_LENGTH_MESSAGE = $"Name cannot exceed {NAME_MAX_LENGTH} characters.";
        public static readonly string FATHERS_SURNAME_MAX_LENGTH_MESSAGE = $"Fathers surname cannot exceed {FATHERS_SURNAME_MAX_LENGTH} characters.";
        public static readonly string MOTHERS_SURNAME_MAX_LENGTH_MESSAGE = $"Mothers surname cannot exceed {MOTHERS_SURNAME_MAX_LENGTH} characters.";
        public static readonly string NATIONALITY_MAX_LENGTH_MESSAGE = $"Nationality cannot exceed {NATIONALITY_MAX_LENGTH} characters.";
    }
}
