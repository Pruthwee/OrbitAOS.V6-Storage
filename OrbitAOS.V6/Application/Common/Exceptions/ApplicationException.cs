// ============================================================
// Application/Common/Exceptions/ApplicationException.cs
// Clean Architecture - Application Layer
// Custom exceptions for application-level error handling
// ============================================================

namespace OrbitAOS.V6.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found.
/// Replaces: returning null from repositories and checking in controllers.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException()
        : base() { }

    public NotFoundException(string message)
        : base(message) { }

    public NotFoundException(string name, object key)
        : base($"Entity '{name}' with key '{key}' was not found.") { }
}

/// <summary>
/// Exception thrown when a validation rule is violated.
/// Works with FluentValidation in the Application layer.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    /// <summary>Dictionary of property names to their validation error messages.</summary>
    public IDictionary<string, string[]> Errors { get; }
}

/// <summary>
/// Exception thrown when the current user is not authorized to perform an action.
/// Replaces: manual authorization checks in legacy controllers.
/// </summary>
public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
        : base("Access to this resource is forbidden.") { }
}
