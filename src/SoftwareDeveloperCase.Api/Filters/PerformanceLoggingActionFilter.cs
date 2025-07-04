using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace SoftwareDeveloperCase.Api.Filters;

/// <summary>
/// Action filter to log API request and response performance metrics
/// </summary>
public class PerformanceLoggingActionFilter : ActionFilterAttribute
{
    private readonly ILogger<PerformanceLoggingActionFilter> _logger;
    private const string StopwatchKey = "ActionStopwatch";

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceLoggingActionFilter"/> class
    /// </summary>
    /// <param name="logger">The logger</param>
    public PerformanceLoggingActionFilter(ILogger<PerformanceLoggingActionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called before the action method is invoked to start performance tracking
    /// </summary>
    /// <param name="context">The action executing context</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        context.HttpContext.Items[StopwatchKey] = stopwatch;

        var controllerName = context.RouteData.Values["controller"];
        var actionName = context.RouteData.Values["action"];
        var method = context.HttpContext.Request.Method;
        var path = context.HttpContext.Request.Path;

        _logger.LogInformation("Starting request {Method} {Path} to {Controller}.{Action}",
            method, path, controllerName, actionName);

        base.OnActionExecuting(context);
    }

    /// <summary>
    /// Called after the action method is executed to log performance metrics
    /// </summary>
    /// <param name="context">The action executed context</param>
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Items[StopwatchKey] is Stopwatch stopwatch)
        {
            stopwatch.Stop();

            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path;
            var statusCode = context.HttpContext.Response.StatusCode;
            var elapsed = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation("Completed request {Method} {Path} to {Controller}.{Action} in {ElapsedMs}ms with status {StatusCode}",
                method, path, controllerName, actionName, elapsed, statusCode);

            // Log performance warning for slow requests
            if (elapsed > 1000) // More than 1 second
            {
                _logger.LogWarning("Slow request detected: {Method} {Path} took {ElapsedMs}ms",
                    method, path, elapsed);
            }

            // Add performance headers
            context.HttpContext.Response.Headers.Append("X-Response-Time-Ms", elapsed.ToString());
        }

        base.OnActionExecuted(context);
    }
}
