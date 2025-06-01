using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SoftwareDeveloperCase.Api.Filters;

/// <summary>
/// Action filter to validate model state and return standardized error responses
/// </summary>
public class ModelValidationActionFilter : ActionFilterAttribute
{
    /// <summary>
    /// Called before the action method is invoked to validate the model state
    /// </summary>
    /// <param name="context">The action executing context</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .SelectMany(x => x.Value?.Errors ?? Enumerable.Empty<Microsoft.AspNetCore.Mvc.ModelBinding.ModelError>())
                .Select(x => x.ErrorMessage)
                .ToArray();

            var response = new
            {
                Type = "ValidationError",
                Title = "One or more validation errors occurred",
                Status = 400,
                Errors = errors,
                TraceId = context.HttpContext.TraceIdentifier
            };

            context.Result = new BadRequestObjectResult(response);
        }

        base.OnActionExecuting(context);
    }
}
