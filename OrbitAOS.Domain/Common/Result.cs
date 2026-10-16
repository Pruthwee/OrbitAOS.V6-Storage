namespace OrbitAOS.Domain.Common
{
    /// <summary>
    /// Represents the result of a domain operation, encapsulating success/failure state and error messages.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; protected set; }

        protected Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, error);
        public static Result<T> Success<T>(T value) => new(value, true, null);
        public static Result<T> Failure<T>(string error) => new(default, false, error);
    }

    /// <summary>
    /// Generic result type carrying a value on success.
    /// </summary>
    /// <typeparam name="T">The type of the value returned on success.</typeparam>
    public class Result<T> : Result
    {
        private readonly T? _value;

        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access Value of a failed result.");

        internal Result(T? value, bool isSuccess, string? error)
            : base(isSuccess, error)
        {
            _value = value;
        }
    }
}
