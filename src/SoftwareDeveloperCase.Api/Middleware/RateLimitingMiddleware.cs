using System.Threading.RateLimiting;

namespace SoftwareDeveloperCase.Api.Middleware;

/// <summary>
/// Middleware for basic rate limiting to prevent API abuse
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly Dictionary<string, TokenBucketRateLimiter> _rateLimiters = new();
    private readonly object _lock = new();

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var rateLimiter = GetRateLimiter(clientId);

        using var lease = await rateLimiter.AcquireAsync(permitCount: 1);
        
        if (!lease.IsAcquired)
        {
            _logger.LogWarning("Rate limit exceeded for client {ClientId}", clientId);
            
            context.Response.StatusCode = 429; // Too Many Requests
            context.Response.Headers.Append("Retry-After", "60");
            
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                Type = "RateLimitExceeded",
                Title = "Rate limit exceeded",
                Status = 429,
                Detail = "Too many requests. Please try again later.",
                TraceId = context.TraceIdentifier
            }));
            
            return;
        }

        // Add rate limit headers
        context.Response.Headers.Append("X-RateLimit-Limit", "100");
        // Use a safer approach to get the remaining tokens
        string remainingTokens = "Unknown";
        if (lease.TryGetMetadata("RemainingTokens", out var tokens) && tokens != null)
        {
            remainingTokens = tokens.ToString() ?? "Unknown";
        }
        context.Response.Headers.Append("X-RateLimit-Remaining", remainingTokens);

        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID from claims
        var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user:{userId}";
        }

        // Fall back to IP address
        var ipAddress = context.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
        return $"ip:{ipAddress}";
    }

    private TokenBucketRateLimiter GetRateLimiter(string clientId)
    {
        lock (_lock)
        {
            if (!_rateLimiters.TryGetValue(clientId, out var rateLimiter))
            {
                var options = new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 100, // Maximum tokens in bucket
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 10, // Queue up to 10 requests
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1), // Replenish every minute
                    TokensPerPeriod = 100, // Add 100 tokens per minute
                    AutoReplenishment = true
                };

                rateLimiter = new TokenBucketRateLimiter(options);
                _rateLimiters[clientId] = rateLimiter;

                // Clean up old rate limiters periodically
                if (_rateLimiters.Count > 1000)
                {
                    var oldest = _rateLimiters.Keys.Take(_rateLimiters.Count / 2).ToList();
                    foreach (var key in oldest)
                    {
                        if (_rateLimiters.TryGetValue(key, out var oldRateLimiter))
                        {
                            oldRateLimiter.Dispose();
                            _rateLimiters.Remove(key);
                        }
                    }
                }
            }

            return rateLimiter;
        }
    }
}
