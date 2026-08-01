namespace Bussines_Library.Application.Common.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }
        public static Result Success() => new Result(true, Error.None);
        public static Result Failure(Error error) => new Result(false, error);
        public static Result ValidationFailure(IReadOnlyDictionary<string, string[]> errors) => new Result(false,new Error("Validation failed.", "One or more validation errors occurred.", ErrorType.Validation), errors);
        
        protected Result(bool isSuccess, Error error, IReadOnlyDictionary<string, string[]>? validationErrors = null)
        {
            if (isSuccess && error.Type != ErrorType.None)
            {
                throw new InvalidOperationException("A successful result cannot contain an error.");
            }

            if (!isSuccess && error.Type == ErrorType.None)
            {
                throw new InvalidOperationException("A failed result must contain an error");
            }

            IsSuccess = isSuccess;
            Error = error;
            ValidationErrors = validationErrors ?? new Dictionary<string, string[]>();

        }
    }

    public sealed class Result<T> : Result
    {
        public T? Value { get; }
        private Result(T? value, bool isSuccess, Error error, IReadOnlyDictionary<string, string[]>? validationErrors = null) : base(isSuccess, error, validationErrors)
        {
            Value = value;
        }
        public static Result<T> Success(T value) => new Result<T>(value, true, Error.None);
        public static new Result<T> Failure(Error error) => new Result<T>(default!, false, error);
        public static new Result<T> ValidationFailure(IReadOnlyDictionary<string, string[]> errors) => new Result<T>(default!, false, new Error("Validation failed.", "One or more validation errors occurred.", ErrorType.Validation), errors);
    }
}
