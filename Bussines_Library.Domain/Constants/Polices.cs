namespace Bussines_Library.Domain.Constants
{
    public static class Polices
    {
        #region Books polices

        public const string BooksRead = "Books.Read";
        public const string BooksWrite = "Books.Write";

        #endregion

        #region Authors polices

        public const string AuthorsRead = "Authors.Read";
        public const string AuthorsWrite = "Authors.Write";
        public const string AuthorsUpdate = "Authors.Update";
        public const string AuthorsActivate = "Authors.Activate";

        #endregion

        #region Loan polices

        public const string LoansRead = "Loans.Read";
        public const string LoansWrite = "Loans.Write";
        public const string LoansReturn = "Loans.Return";

        #endregion
    }
}
