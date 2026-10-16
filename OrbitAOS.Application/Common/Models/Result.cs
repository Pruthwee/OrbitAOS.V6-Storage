namespace OrbitAOS.Application.Common.Models;

/// <summary>
/// Represents the result of an application operation, encapsulating success/failure state and messages.
/// </summary>
public class Result
{
    /// <summary>Gets whether the operation succeeded.</summary>
    public bool Succeeded { get; private set; }

    /// <summary>Gets the list of error messages if the operation failed.</summary>
    public IReadOnlyList<string> Errors { get; private set; }

    /// <summary>Initializes a new instance of Result.</summary>
    protected Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToList().AsReadOnly();
    }

    /// <summary>Creates a successful result.</summary>
    public static Result Success() => new(true, Array.Empty<string>());

    /// <summary>Creates a failed result with the specified errors.</summary>
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);

    /// <summary>Creates a failed result with a single error message.</summary>
    public static Result Failure(string error) => new(false, new[] { error });
}

/// <summary>
/// Represents the result of an application operation that returns a value.
/// </summary>
/// <typeparam name="T">The type of the returned value.</typeparam>
public class Result<T> : Result
{
    /// <summary>Gets the value returned by the operation (null if failed).</summary>
    public T? Value { get; private set; }

    /// <summary>Initializes a new instance of Result with a value.</summary>
    private Result(bool succeeded, T? value, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Value = value;
    }

    /// <summary>Creates a successful result with the specified value.</summary>
    public static Result<T> Success(T value) => new(true, value, Array.Empty<string>());

    /// <summary>Creates a failed result with the specified errors.</summary>
    public new static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);

    /// <summary>Creates a failed result with a single error message.</summary>
    public new static Result<T> Failure(string error) => new(false, default, new[] { error });
}
