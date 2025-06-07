#nullable enable

using System.Reflection;
using FluentValidation;
using MediatR;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Application.Behaviours;

/// <summary>
/// Pipeline behavior that validates requests and handles Result{T} responses
/// </summary>
/// <typeparam name="TRequest">The type of request being validated</typeparam>
/// <typeparam name="TResponse">The type of response being returned</typeparam>
public class ResultValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the ResultValidationBehaviour class
    /// </summary>
    /// <param name="validators">Collection of validators for the request type</param>
    public ResultValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles the request pipeline and validates the request before proceeding
    /// </summary>
    /// <param name="request">The request being validated</param>
    /// <param name="next">The next handler in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The response from the next handler or a validation failure result</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                // Check if TResponse is a Result or Result<T>
                if (IsResultType(typeof(TResponse)))
                {
                    var validationResult = CreateValidationFailureResult<TResponse>(failures);
                    return validationResult;
                }

                // Fall back to throwing exception for non-Result types
                throw new Exceptions.ValidationException(failures);
            }
        }

        return await next();
    }

    /// <summary>
    /// Checks if the type is a Result or Result{T}
    /// </summary>
    /// <param name="type">The type to check</param>
    /// <returns>True if the type is a Result type</returns>
    private static bool IsResultType(Type type)
    {
        if (type == typeof(Result))
            return true;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<>))
            return true;

        return false;
    }

    /// <summary>
    /// Creates a validation failure result for the specified response type
    /// </summary>
    /// <typeparam name="T">The response type</typeparam>
    /// <param name="failures">The validation failures</param>
    /// <returns>A Result or Result{T} with validation errors</returns>
    private static T CreateValidationFailureResult<T>(IList<FluentValidation.Results.ValidationFailure> failures)
    {
        var errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());

        var responseType = typeof(T);

        if (responseType == typeof(Result))
        {
            var result = Result.Invalid(errors);
            return (T)(object)result;
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var invalidMethod = typeof(Result<>).MakeGenericType(valueType).GetMethod("Invalid", new[] { typeof(IDictionary<string, string[]>) });

            if (invalidMethod != null)
            {
                var result = invalidMethod.Invoke(null, new object[] { errors });
                return (T)result!;
            }
        }

        // This should never happen if IsResultType check is correct
        throw new InvalidOperationException($"Cannot create validation failure result for type {responseType}");
    }
}
