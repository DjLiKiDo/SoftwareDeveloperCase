#nullable enable

using Microsoft.AspNetCore.Mvc;
using SoftwareDeveloperCase.Application.Models;

namespace SoftwareDeveloperCase.Api.Controllers;

/// <summary>
/// Base controller that provides Result{T} handling capabilities
/// </summary>
public class BaseController : ControllerBase
{
    /// <summary>
    /// Converts a Result to an appropriate IActionResult
    /// </summary>
    /// <param name="result">The result to convert</param>
    /// <returns>An IActionResult representing the result</returns>
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            if (result.Warnings?.Any() == true)
            {
                return Ok(new { success = true, warnings = result.Warnings });
            }
            return NoContent();
        }

        return HandleFailureResult(result);
    }

    /// <summary>
    /// Converts a Result{T} to an appropriate ActionResult{T}
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="result">The result to convert</param>
    /// <returns>An ActionResult{T} representing the result</returns>
    protected ActionResult<T> HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (result.Value != null)
            {
                if (result.Warnings?.Any() == true)
                {
                    // Return the value with warnings in response headers or wrapped response
                    Response.Headers["X-Warnings"] = string.Join("; ", result.Warnings);
                }
                return Ok(result.Value);
            }

            // Handle null value success case
            if (result.Warnings?.Any() == true)
            {
                return Ok(new { success = true, warnings = result.Warnings });
            }
            return NoContent();
        }

        return HandleFailureResult<T>(result);
    }

    /// <summary>
    /// Converts a Result{T} to an appropriate IActionResult
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="result">The result to convert</param>
    /// <returns>An IActionResult representing the result</returns>
    protected IActionResult HandleResultAsAction<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            if (result.Value != null)
            {
                if (result.Warnings?.Any() == true)
                {
                    Response.Headers["X-Warnings"] = string.Join("; ", result.Warnings);
                }
                return Ok(result.Value);
            }

            // Handle null value success case
            if (result.Warnings?.Any() == true)
            {
                return Ok(new { success = true, warnings = result.Warnings });
            }
            return NoContent();
        }

        return HandleFailureResult(result);
    }

    /// <summary>
    /// Converts a Result{T} to an appropriate ActionResult{T} for creation scenarios
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="result">The result to convert</param>
    /// <param name="actionName">The action name for the location header</param>
    /// <param name="routeValues">The route values for the location header</param>
    /// <returns>An ActionResult{T} representing the result</returns>
    protected ActionResult<T> HandleCreatedResult<T>(Result<T> result, string actionName, object? routeValues = null)
    {
        if (result.IsSuccess && result.Value != null)
        {
            if (result.Warnings?.Any() == true)
            {
                Response.Headers["X-Warnings"] = string.Join("; ", result.Warnings);
            }
            return CreatedAtAction(actionName, routeValues, result.Value);
        }

        if (result.IsSuccess)
        {
            // Success but no value - this shouldn't happen for creation
            return BadRequest("Creation successful but no value returned");
        }

        return HandleFailureResult<T>(result);
    }

    /// <summary>
    /// Converts a Result{T} to an appropriate IActionResult for creation scenarios
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="result">The result to convert</param>
    /// <param name="actionName">The action name for the location header</param>
    /// <param name="routeValues">The route values for the location header</param>
    /// <returns>An IActionResult representing the result</returns>
    protected IActionResult HandleCreatedResultAsAction<T>(Result<T> result, string actionName, object? routeValues = null)
    {
        if (result.IsSuccess && result.Value != null)
        {
            if (result.Warnings?.Any() == true)
            {
                Response.Headers["X-Warnings"] = string.Join("; ", result.Warnings);
            }
            return CreatedAtAction(actionName, routeValues, result.Value);
        }

        if (result.IsSuccess)
        {
            // Success but no value - this shouldn't happen for creation
            return BadRequest("Creation successful but no value returned");
        }

        return HandleFailureResult(result);
    }

    /// <summary>
    /// Handles failure results and returns appropriate error responses
    /// </summary>
    /// <param name="result">The failed result</param>
    /// <returns>An IActionResult representing the error</returns>
    private IActionResult HandleFailureResult(Result result)
    {
        // Handle validation errors
        if (result.ValidationErrors?.Any() == true)
        {
            foreach (var error in result.ValidationErrors)
            {
                foreach (var message in error.Value)
                {
                    ModelState.AddModelError(error.Key, message);
                }
            }
            return ValidationProblem(ModelState);
        }

        // Determine appropriate error response based on error message
        var errorMessage = result.Error ?? "An error occurred";

        // Authentication errors (Unauthorized)
        if (errorMessage.Contains("invalid email or password", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("invalid credentials", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("authentication failed", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("invalid token", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("token expired", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("account is temporarily locked", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(new { message = errorMessage });
        }

        // Not Found patterns
        if (errorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new { message = errorMessage });
        }

        // Business rule violations (Unprocessable Entity)
        if (errorMessage.Contains("cannot", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("not allowed", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("invalid operation", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("business rule", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(new { message = errorMessage });
        }

        // Default to BadRequest
        return BadRequest(new { message = errorMessage });
    }

    /// <summary>
    /// Handles failure results and returns appropriate error responses as ActionResult{T}
    /// </summary>
    /// <typeparam name="T">The type parameter for ActionResult{T}</typeparam>
    /// <param name="result">The failed result</param>
    /// <returns>An ActionResult{T} representing the error</returns>
    private ActionResult<T> HandleFailureResult<T>(Result result)
    {
        // Handle validation errors
        if (result.ValidationErrors?.Any() == true)
        {
            foreach (var error in result.ValidationErrors)
            {
                foreach (var message in error.Value)
                {
                    ModelState.AddModelError(error.Key, message);
                }
            }
            return ValidationProblem(ModelState);
        }

        // Determine appropriate error response based on error message
        var errorMessage = result.Error ?? "An error occurred";

        // Authentication errors (Unauthorized)
        if (errorMessage.Contains("invalid email or password", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("invalid credentials", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("authentication failed", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("unauthorized", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("invalid token", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("token expired", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("account is temporarily locked", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(new { message = errorMessage });
        }

        // Not Found patterns
        if (errorMessage.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("does not exist", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new { message = errorMessage });
        }

        // Business rule violations (Unprocessable Entity)
        if (errorMessage.Contains("cannot", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("not allowed", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("invalid operation", StringComparison.OrdinalIgnoreCase) ||
            errorMessage.Contains("business rule", StringComparison.OrdinalIgnoreCase))
        {
            return UnprocessableEntity(new { message = errorMessage });
        }

        // Default to BadRequest
        return BadRequest(new { message = errorMessage });
    }

    /// <summary>
    /// Sets the result in HttpContext.Items for middleware processing
    /// This is an alternative approach that can be used with the ResultResponseMiddleware
    /// </summary>
    /// <param name="result">The result to store</param>
    /// <returns>An EmptyResult to prevent further processing</returns>
    protected IActionResult StoreResult(Result result)
    {
        HttpContext.Items["Result"] = result;
        return new EmptyResult();
    }

    /// <summary>
    /// Sets the result in HttpContext.Items for middleware processing
    /// This is an alternative approach that can be used with the ResultResponseMiddleware
    /// </summary>
    /// <typeparam name="T">The type of the result value</typeparam>
    /// <param name="result">The result to store</param>
    /// <returns>An EmptyResult to prevent further processing</returns>
    protected IActionResult StoreResult<T>(Result<T> result)
    {
        HttpContext.Items["Result"] = result;
        return new EmptyResult();
    }
}
