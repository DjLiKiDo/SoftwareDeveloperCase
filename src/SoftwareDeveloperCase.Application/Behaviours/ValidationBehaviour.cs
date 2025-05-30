using FluentValidation;
using MediatR;

namespace SoftwareDeveloperCase.Application.Behaviours;

/// <summary>
/// Pipeline behavior that validates requests using FluentValidation before they reach their handlers
/// </summary>
/// <typeparam name="TRequest">The type of request being validated</typeparam>
/// <typeparam name="TResponse">The type of response being returned</typeparam>
public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the ValidationBehaviour class
    /// </summary>
    /// <param name="validators">Collection of validators for the request type</param>
    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles the request pipeline and validates the request before proceeding
    /// </summary>
    /// <param name="request">The request being validated</param>
    /// <param name="next">The next handler in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The response from the next handler</returns>
    /// <exception cref="ValidationException">Thrown when validation fails</exception>
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
                throw new Exceptions.ValidationException(failures);
            }
        }

        return await next();
    }
}
