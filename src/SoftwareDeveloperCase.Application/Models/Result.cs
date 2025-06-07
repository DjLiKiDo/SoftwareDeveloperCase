#nullable enable

namespace SoftwareDeveloperCase.Application.Models;

/// <summary>
/// Represents the result of an operation that can either succeed or fail
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the operation failed
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// Gets the validation errors if validation failed
    /// </summary>
    public IDictionary<string, string[]>? ValidationErrors { get; private set; }

    /// <summary>
    /// Gets the warning messages if any
    /// </summary>
    public IList<string>? Warnings { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Result class
    /// </summary>
    /// <param name="isSuccess">Whether the operation was successful</param>
    /// <param name="error">The error message if failed</param>
    /// <param name="validationErrors">The validation errors if validation failed</param>
    /// <param name="warnings">Any warning messages</param>
    protected Result(bool isSuccess, string? error = null, IDictionary<string, string[]>? validationErrors = null, IList<string>? warnings = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        ValidationErrors = validationErrors;
        Warnings = warnings;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    /// <returns>A successful result</returns>
    public static Result Success() => new(true);

    /// <summary>
    /// Creates a successful result with warnings
    /// </summary>
    /// <param name="warnings">Warning messages</param>
    /// <returns>A successful result with warnings</returns>
    public static Result Success(IList<string> warnings) => new(true, warnings: warnings);

    /// <summary>
    /// Creates a failed result with an error message
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failed result</returns>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Creates a failed result with an optional error message and warnings
    /// </summary>
    /// <param name="error">The error message</param>
    /// <param name="warnings">Any warning messages</param>
    /// <returns>A failed result</returns>
    public static Result Failure(string error, IList<string>? warnings) => new(false, error, warnings: warnings);

    /// <summary>
    /// Creates a failed result for not found scenarios
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failed result for not found</returns>
    public static Result NotFound(string error) => new(false, error);

    /// <summary>
    /// Creates a failed result for validation errors
    /// </summary>
    /// <param name="validationErrors">The validation errors</param>
    /// <returns>A failed result with validation errors</returns>
    public static Result Invalid(IDictionary<string, string[]> validationErrors) => new(false, validationErrors: validationErrors);

    /// <summary>
    /// Creates a validation failed result with optional warnings
    /// </summary>
    /// <param name="validationErrors">The validation errors</param>
    /// <param name="warnings">Any warning messages</param>
    /// <returns>A validation failed result</returns>
    public static Result Invalid(IDictionary<string, string[]> validationErrors, IList<string>? warnings) => new(false, validationErrors: validationErrors, warnings: warnings);

    /// <summary>
    /// Creates a failed result for validation errors with a general error message
    /// </summary>
    /// <param name="error">The general error message</param>
    /// <param name="validationErrors">The validation errors</param>
    /// <returns>A failed result with validation errors</returns>
    public static Result Invalid(string error, IDictionary<string, string[]> validationErrors) => new(false, error, validationErrors);

    /// <summary>
    /// Implicit conversion from boolean to Result
    /// </summary>
    /// <param name="success">Whether the operation was successful</param>
    public static implicit operator Result(bool success) => success ? Success() : Failure("Operation failed");

    /// <summary>
    /// Implicit conversion from string to Result (treated as error)
    /// </summary>
    /// <param name="error">The error message</param>
    public static implicit operator Result(string error) => Failure(error);
}

/// <summary>
/// Represents the result of an operation that can either succeed with a value or fail
/// </summary>
/// <typeparam name="T">The type of the value returned on success</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Gets the value if the operation was successful
    /// </summary>
    public T? Value { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Result{T} class
    /// </summary>
    /// <param name="isSuccess">Whether the operation was successful</param>
    /// <param name="value">The value if successful</param>
    /// <param name="error">The error message if failed</param>
    /// <param name="validationErrors">The validation errors if validation failed</param>
    /// <param name="warnings">Any warning messages</param>
    private Result(bool isSuccess, T? value = default, string? error = null, IDictionary<string, string[]>? validationErrors = null, IList<string>? warnings = null)
        : base(isSuccess, error, validationErrors, warnings)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a successful result with a value
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>A successful result with the value</returns>
    public static Result<T> Success(T value) => new(true, value);

    /// <summary>
    /// Creates a successful result with a value and warnings
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="warnings">Warning messages</param>
    /// <returns>A successful result with the value and warnings</returns>
    public static Result<T> Success(T value, IList<string> warnings) => new(true, value, warnings: warnings);

    /// <summary>
    /// Creates a failed result with an error message
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failed result</returns>
    public static new Result<T> Failure(string error) => new(false, error: error);

    /// <summary>
    /// Creates a failed result with an optional error message and warnings
    /// </summary>
    /// <param name="error">The error message</param>
    /// <param name="warnings">Any warning messages</param>
    /// <returns>A failed result</returns>
    public static new Result<T> Failure(string error, IList<string>? warnings) => new(false, error: error, warnings: warnings);

    /// <summary>
    /// Creates a failed result for not found scenarios
    /// </summary>
    /// <param name="error">The error message</param>
    /// <returns>A failed result for not found</returns>
    public static new Result<T> NotFound(string error) => new(false, error: error);

    /// <summary>
    /// Creates a failed result for validation errors
    /// </summary>
    /// <param name="validationErrors">The validation errors</param>
    /// <returns>A failed result with validation errors</returns>
    public static new Result<T> Invalid(IDictionary<string, string[]> validationErrors) => new(false, validationErrors: validationErrors);

    /// <summary>
    /// Creates a validation failed result with optional warnings
    /// </summary>
    /// <param name="validationErrors">The validation errors</param>
    /// <param name="warnings">Any warning messages</param>
    /// <returns>A validation failed result</returns>
    public static new Result<T> Invalid(IDictionary<string, string[]> validationErrors, IList<string>? warnings) => new(false, validationErrors: validationErrors, warnings: warnings);

    /// <summary>
    /// Creates a failed result for validation errors with a general error message
    /// </summary>
    /// <param name="error">The general error message</param>
    /// <param name="validationErrors">The validation errors</param>
    /// <returns>A failed result with validation errors</returns>
    public static new Result<T> Invalid(string error, IDictionary<string, string[]> validationErrors) => new(false, error: error, validationErrors: validationErrors);

    /// <summary>
    /// Implicit conversion from T to Result{T}
    /// </summary>
    /// <param name="value">The value</param>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicit conversion from string to Result{T} (treated as error)
    /// </summary>
    /// <param name="error">The error message</param>
    public static implicit operator Result<T>(string error) => Failure(error);

}
