// SalonManagement.API/Domain/Common/Result.cs
namespace SalonManagement.API.Domain.Common
{
    /// <summary>
    /// Result pattern for operation outcomes
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public string? Error { get; }           // <-- nullable
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string? error)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Success result cannot have an error");

            if (!isSuccess && string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Failure result must have an error");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, null);
        public static Result Failure(string error) => new Result(false, error);

        // Generic helpers: call like Result.Success<T>(value) or Result.Failure<T>(error)
        public static Result<T> Success<T>(T value) => new Result<T>(value, true, null);
        public static Result<T> Failure<T>(string error) => new Result<T>(default!, false, error);
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        // Keep the constructor internal/protected so creation goes through the static helpers
        protected internal Result(T value, bool isSuccess, string? error)
            : base(isSuccess, error)
        {
            Value = value;
        }
    }
}








//// SalonManagement.API/Domain/Common/Result.cs
//namespace SalonManagement.API.Domain.Common
//{
//    /// <summary>
//    /// Result pattern for operation outcomes
//    /// </summary>
//    public class Result
//    {
//        public bool IsSuccess { get; }
//        public string Error { get; }
//        public bool IsFailure => !IsSuccess;

//        protected Result(bool isSuccess, string error)
//        {
//            if (isSuccess && !string.IsNullOrEmpty(error))
//                throw new InvalidOperationException("Success result cannot have an error");

//            if (!isSuccess && string.IsNullOrEmpty(error))
//                throw new InvalidOperationException("Failure result must have an error");

//            IsSuccess = isSuccess;
//            Error = error;
//        }

//        public static Result Success() => new Result(true, null);
//        public static Result Failure(string error) => new Result(false, error);
//        public static Result<T> Success<T>(T value) => new Result<T>(value, true, null);
//        public static Result<T> Failure<T>(string error) => new Result<T>(default, false, error);
//    }

//    public class Result<T> : Result
//    {
//        public T Value { get; }

//        protected internal Result(T value, bool isSuccess, string error)
//            : base(isSuccess, error)
//        {
//            Value = value;
//        }
//    }
//}