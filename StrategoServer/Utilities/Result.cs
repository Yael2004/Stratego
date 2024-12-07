using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Result<T>
    {
        public T Value { get; private set; }
        public bool IsSuccess { get; private set; }
        public string Error { get; private set; }
        public bool IsDataBaseError { get; private set; }

        public Result(T value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = isSuccess ? string.Empty : error;
            IsDataBaseError = false;
        }

        public Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = isSuccess ? string.Empty : error;
            IsDataBaseError = false;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(default, false, error);
        }

        public static Result<T> DataBaseError(string error)
        {
            return new Result<T>(default, false, error) { IsDataBaseError = true };
        }
    }
}