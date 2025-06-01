using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using SoftwareDeveloperCase.Api.Middleware;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace SoftwareDeveloperCase.Test.Unit.Api.Middleware;

public class RequestSanitizationMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldSanitizeQueryStringParameters()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Add query string parameters with potentially harmful content
        var queryCollection = new Dictionary<string, StringValues>
        {
            { "search", new StringValues("<script>alert('XSS')</script>test") },
            { "filter", new StringValues(new[] { "normal", "<img onerror=alert('xss') src=x>" }) }
        };

        context.Request.Query = new QueryCollection(queryCollection);

        var requestDelegate = new RequestDelegate(_ => Task.CompletedTask);
        var middleware = new RequestSanitizationMiddleware(requestDelegate);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var sanitizedSearch = context.Request.Query["search"].ToString();
        var sanitizedFilters = context.Request.Query["filter"].ToArray();

        Assert.Equal("&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;test", sanitizedSearch);
        Assert.Equal("normal", sanitizedFilters[0]);
        Assert.Equal("&lt;img onerror=alert(&#39;xss&#39;) src=x&gt;", sanitizedFilters[1]);
    }

    [Fact]
    public async Task InvokeAsync_WithNoQueryParams_ShouldNotModifyRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var requestDelegate = new RequestDelegate(_ => Task.CompletedTask);
        var middleware = new RequestSanitizationMiddleware(requestDelegate);

        // Act
        await middleware.InvokeAsync(context);

        // Assert - No exception should be thrown
        Assert.Empty(context.Request.Query);
    }

    [Fact]
    public async Task InvokeAsync_WithEmptyQueryParams_ShouldNotModifyValue()
    {
        // Arrange
        var context = new DefaultHttpContext();

        // Add empty query string parameter
        var queryCollection = new Dictionary<string, StringValues>
        {
            { "empty", new StringValues(string.Empty) }
        };

        context.Request.Query = new QueryCollection(queryCollection);

        var requestDelegate = new RequestDelegate(_ => Task.CompletedTask);
        var middleware = new RequestSanitizationMiddleware(requestDelegate);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(string.Empty, context.Request.Query["empty"].ToString());
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var nextInvoked = false;

        var requestDelegate = new RequestDelegate(_ =>
        {
            nextInvoked = true;
            return Task.CompletedTask;
        });

        var middleware = new RequestSanitizationMiddleware(requestDelegate);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextInvoked, "Next middleware was not called");
    }
}
