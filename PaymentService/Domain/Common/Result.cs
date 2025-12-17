namespace PaymentService.Domain.Common;

public struct None
{
    public static readonly None Value = new();
}
public class Result
{
    protected Result()
    {
        Errors = new List<string>();
    }

    public bool Succeeded { get; set; }
    public List<string> Errors { get; set; }

    public Result WithMessage(string message)
    {
        Errors.Add(message);
        return this;
    }
}

public class Result<T> : Result
{

    public T Data { get; set; }

    //public List<ValidationResult> ValidationErrors { get; set; }


    #region Non Async Methods

    #region Success Methods

    public static Result<T> Success()
    {
        return new Result<T>
        {
            Succeeded = true
        };
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            Succeeded = true,
            Data = data
        };
    }

    public static Result<T> Success(T data, string message)
    {
        return new Result<T>
        {
            Succeeded = true,
            Errors = new List<string> { message },
            Data = data
        };
    }

    #endregion

    #region Failure Methods

    public static Result<T> Failure()
    {
        return new Result<T>
        {
            Succeeded = false
        };
    }

    public static Result<T> Failure(string message)
    {
        return new Result<T>
        {
            Succeeded = false,
            Errors = new List<string> { message }
        };
    }

    public static Result<T> Failure(List<string> messages)
    {
        return new Result<T>
        {
            Succeeded = false,
            Errors = messages
        };
    }

    public static Result<T> Failure(T data)
    {
        return new Result<T>
        {
            Succeeded = false,
            Data = data
        };
    }

    public static Result<T> Failure(T data, string message)
    {
        return new Result<T>
        {
            Succeeded = false,
            Errors = new List<string> { message },
            Data = data
        };
    }

    public static Result<T> Failure(T data, List<string> messages)
    {
        return new Result<T>
        {
            Succeeded = false,
            Errors = messages,
            Data = data
        };
    }

    public static Result<T> Failure(Exception exception)
    {
        return new Result<T>
        {
            Succeeded = false,
        };
    }

    #endregion

    #endregion

    #region Async Methods

    #region Success Methods

    public static Task<Result<T>> SuccessAsync()
    {
        return Task.FromResult(Success());
    }

    public static Task<Result<T>> SuccessAsync(T data)
    {
        return Task.FromResult(Success(data));
    }

    public static Task<Result<T>> SuccessAsync(T data, string message)
    {
        return Task.FromResult(Success(data, message));
    }

    #endregion

    #region Failure Methods

    public static Task<Result<T>> FailureAsync()
    {
        return Task.FromResult(Failure());
    }

    public static Task<Result<T>> FailureAsync(string message)
    {
        return Task.FromResult(Failure(message));
    }

    public static Task<Result<T>> FailureAsync(List<string> messages)
    {
        return Task.FromResult(Failure(messages));
    }

    public static Task<Result<T>> FailureAsync(T data)
    {
        return Task.FromResult(Failure(data));
    }

    public static Task<Result<T>> FailureAsync(T data, string message)
    {
        return Task.FromResult(Failure(data, message));
    }

    public static Task<Result<T>> FailureAsync(T data, List<string> messages)
    {
        return Task.FromResult(Failure(data, messages));
    }

    public static Task<Result<T>> FailureAsync(Exception exception)
    {
        return Task.FromResult(Failure(exception));
    }

    #endregion

    #endregion

}
