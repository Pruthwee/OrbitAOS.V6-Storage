// ============================================================
// Application/Common/Models/Result.cs
// Clean Architecture - Application Layer
// Result pattern for operation outcomes (replaces exception-only error handling)
// ============================================================

namespace OrbitAOS.V6.Application.Common.Models;

/// <summary>
/// Represents the outcome of an application operation.
/// Provides a structured way to return success/failure without exceptions.
/// Used by MediatR command/query handlers in the Application layer.
/// </summary>
public class Result
{
    protected Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    /// <summary>Indicates whether the operation succeeded.</summary>
    public bool Succeeded { get; }

    /// <summary>Collection of error messages if the operation failed.</summary>
    public string[] Errors { get; }

    /// <summary>Creates a successful result.</summary>
    public static Result Success() => new(true, Array.Empty<string>());

    /// <summary>Creates a failed result with error messages.</summary>
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);

    /// <summary>Creates a failed result with a single error message.</summary>
    public static Result Failure(string error) => new(false, new[] { error });
}

/// <summary>
/// Generic result carrying a value on success.
/// </summary>
/// <typeparam name="T">Type of the result value.</typeparam>
public class Result<T> : Result
{
    private Result(bool succeeded, T? value, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Value = value;
    }

    /// <summary>The result value (null on failure).</summary>
    public T? Value { get; }

    /// <summary>Creates a successful result with a value.</summary>
    public static Result<T> Success(T value) => new(true, value, Array.Empty<string>());

    /// <summary>Creates a failed result with error messages.</summary>
    public new static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);

    /// <summary>Creates a failed result with a single error message.</summary>
    public new static Result<T> Failure(string error) => new(false, default, new[] { error });
}
