#nullable enable

using FluentValidation.Results;
using SoftwareDeveloperCase.Application.Exceptions;

namespace SoftwareDeveloperCase.Application.Models;

/// <summary>
/// Extension methods for Result and Result{T} to integrate with existing exception patterns
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Creates a Result from a ValidationException
    /// </summary>
    /// <param name="validationException">The validation exception</param>
    /// <returns>A failed result with validation errors</returns>
    public static Result FromValidationException(ValidationException validationException)
        => Result.Invalid(validationException.Errors);

    /// <summary>
    /// Creates a Result{T} from a ValidationException
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="validationException">The validation exception</param>
    /// <returns>A failed result with validation errors</returns>
    public static Result<T> FromValidationException<T>(ValidationException validationException)
        => Result<T>.Invalid(validationException.Errors);

    /// <summary>
    /// Creates a Result from a NotFoundException
    /// </summary>
    /// <param name="notFoundException">The not found exception</param>
    /// <returns>A failed result for not found</returns>
    public static Result FromNotFoundException(NotFoundException notFoundException)
        => Result.NotFound(notFoundException.Message);

    /// <summary>
    /// Creates a Result{T} from a NotFoundException
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="notFoundException">The not found exception</param>
    /// <returns>A failed result for not found</returns>
    public static Result<T> FromNotFoundException<T>(NotFoundException notFoundException)
        => Result<T>.NotFound(notFoundException.Message);

    /// <summary>
    /// Creates a Result from a BusinessRuleViolationException
    /// </summary>
    /// <param name="businessException">The business rule violation exception</param>
    /// <returns>A failed result with business error</returns>
    public static Result FromBusinessException(BusinessRuleViolationException businessException)
        => Result.Failure(businessException.Message);

    /// <summary>
    /// Creates a Result{T} from a BusinessRuleViolationException
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="businessException">The business rule violation exception</param>
    /// <returns>A failed result with business error</returns>
    public static Result<T> FromBusinessException<T>(BusinessRuleViolationException businessException)
        => Result<T>.Failure(businessException.Message);

    /// <summary>
    /// Creates a Result from FluentValidation failures
    /// </summary>
    /// <param name="validationFailures">The validation failures</param>
    /// <returns>A failed result with validation errors</returns>
    public static Result FromValidationFailures(IEnumerable<ValidationFailure> validationFailures)
    {
        var errors = validationFailures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

        return Result.Invalid(errors);
    }

    /// <summary>
    /// Creates a Result{T} from FluentValidation failures
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="validationFailures">The validation failures</param>
    /// <returns>A failed result with validation errors</returns>
    public static Result<T> FromValidationFailures<T>(IEnumerable<ValidationFailure> validationFailures)
    {
        var errors = validationFailures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

        return Result<T>.Invalid(errors);
    }

    /// <summary>
    /// Executes an action and converts exceptions to Result
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <returns>A result representing the outcome</returns>
    public static Result Try(Action action)
    {
        try
        {
            action();
            return Result.Success();
        }
        catch (ValidationException ex)
        {
            return FromValidationException(ex);
        }
        catch (NotFoundException ex)
        {
            return FromNotFoundException(ex);
        }
        catch (BusinessRuleViolationException ex)
        {
            return FromBusinessException(ex);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Executes a function and converts exceptions to Result{T}
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="func">The function to execute</param>
    /// <returns>A result representing the outcome</returns>
    public static Result<T> Try<T>(Func<T> func)
    {
        try
        {
            var result = func();
            return Result<T>.Success(result);
        }
        catch (ValidationException ex)
        {
            return FromValidationException<T>(ex);
        }
        catch (NotFoundException ex)
        {
            return FromNotFoundException<T>(ex);
        }
        catch (BusinessRuleViolationException ex)
        {
            return FromBusinessException<T>(ex);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Executes an async function and converts exceptions to Result{T}
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="func">The async function to execute</param>
    /// <returns>A task containing a result representing the outcome</returns>
    public static async Task<Result<T>> TryAsync<T>(Func<Task<T>> func)
    {
        try
        {
            var result = await func();
            return Result<T>.Success(result);
        }
        catch (ValidationException ex)
        {
            return FromValidationException<T>(ex);
        }
        catch (NotFoundException ex)
        {
            return FromNotFoundException<T>(ex);
        }
        catch (BusinessRuleViolationException ex)
        {
            return FromBusinessException<T>(ex);
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Executes an async action and converts exceptions to Result
    /// </summary>
    /// <param name="action">The async action to execute</param>
    /// <returns>A task containing a result representing the outcome</returns>
    public static async Task<Result> TryAsync(Func<Task> action)
    {
        try
        {
            await action();
            return Result.Success();
        }
        catch (ValidationException ex)
        {
            return FromValidationException(ex);
        }
        catch (NotFoundException ex)
        {
            return FromNotFoundException(ex);
        }
        catch (BusinessRuleViolationException ex)
        {
            return FromBusinessException(ex);
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
