using MediatR;
using Microsoft.Extensions.Logging;

namespace SoftwareDeveloperCase.Application.Behaviours;

/// <summary>
/// Pipeline behavior that handles unhandled exceptions from request handlers and logs them
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
/// <typeparam name="TResponse">The type of response being returned</typeparam>
public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    /// <summary>
    /// Initializes a new instance of the UnhandledExceptionBehaviour class
    /// </summary>
    /// <param name="logger">The logger instance for logging unhandled exceptions</param>
    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the request pipeline and catches any unhandled exceptions
    /// </summary>
    /// <param name="request">The request being processed</param>
    /// <param name="next">The next handler in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The response from the next handler</returns>
    /// <exception cref="Exception">Re-throws any caught exception after logging</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogError(ex, "Unhandled Exception for Request {Name} {@Request}", requestName, request);
            throw;
        }
    }
}
