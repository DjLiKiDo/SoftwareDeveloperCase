using MediatR;
using System.Collections.Concurrent;
using System.Reflection;
using SoftwareDeveloperCase.Application.Services;
using SoftwareDeveloperCase.Application.Attributes;

namespace SoftwareDeveloperCase.Application.Behaviours;

/// <summary>
/// Pipeline behavior that sanitizes string properties in request objects before they reach their handlers
/// </summary>
/// <typeparam name="TRequest">The type of request being sanitized</typeparam>
/// <typeparam name="TResponse">The type of response being returned</typeparam>
public class SanitizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Cache for property reflection metadata to improve performance
    /// </summary>
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();
    /// <summary>
    /// Handles the request pipeline and sanitizes string properties before proceeding
    /// </summary>
    /// <param name="request">The request being sanitized</param>
    /// <param name="next">The next handler in the pipeline</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The response from the next handler</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Skip sanitization for null requests
        if (request == null)
        {
            return await next();
        }

        // Sanitize all string properties recursively
        SanitizeProperties(request);

        // Continue with the pipeline
        return await next();
    }

    /// <summary>
    /// Recursively sanitizes all string properties in an object
    /// </summary>
    /// <param name="obj">The object whose properties need to be sanitized</param>
    private static void SanitizeProperties(object obj)
    {
        // Skip null objects or primitive types
        if (obj == null || obj.GetType().IsPrimitive || obj is string)
        {
            return;
        }

        // Get all properties of the object using cached reflection metadata
        var objectType = obj.GetType();
        var properties = PropertyCache.GetOrAdd(objectType,
            type => type.GetProperties(BindingFlags.Public | BindingFlags.Instance));

        foreach (var property in properties)
        {
            // Check if property has SkipSanitizationAttribute
            if (property.GetCustomAttribute<SkipSanitizationAttribute>() != null)
            {
                continue; // Skip sanitization for this property
            }

            // Handle string properties
            if (property.PropertyType == typeof(string) && property.CanWrite)
            {
                var value = property.GetValue(obj) as string;

                // Skip null or empty strings
                if (!string.IsNullOrEmpty(value))
                {
                    var sanitized = InputSanitizer.SanitizeString(value);
                    property.SetValue(obj, sanitized);
                }
            }
            // Handle collection properties
            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) &&
                    property.PropertyType != typeof(string))
            {
                var collection = property.GetValue(obj) as System.Collections.IEnumerable;

                if (collection != null)
                {
                    foreach (var item in collection)
                    {
                        if (item != null && !item.GetType().IsPrimitive && !(item is string))
                        {
                            SanitizeProperties(item);
                        }
                    }
                }
            }
            // Recursively handle complex objects
            else if (!property.PropertyType.IsPrimitive &&
                    property.PropertyType != typeof(decimal) &&
                    property.PropertyType != typeof(DateTime) &&
                    property.PropertyType != typeof(DateTimeOffset) &&
                    property.PropertyType != typeof(TimeSpan) &&
                    property.PropertyType != typeof(Guid))
            {
                var value = property.GetValue(obj);
                if (value != null)
                {
                    SanitizeProperties(value);
                }
            }
        }
    }
}
