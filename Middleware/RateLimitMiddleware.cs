using System.Collections.Concurrent;

namespace AutoSalon.Middleware;

/// <summary>
/// Rate limiter: не более 5 POST-запросов в минуту с одного IP на /leads
/// </summary>
public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitMiddleware> _logger;

    // IP → список timestamp'ов запросов
    private static readonly ConcurrentDictionary<string, List<DateTime>> _store = new();
    private const int MaxRequests = 5;
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "POST" &&
            context.Request.Path.StartsWithSegments("/leads"))
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;

            var timestamps = _store.GetOrAdd(ip, _ => new List<DateTime>());

            lock (timestamps)
            {
                // Чистим старые записи за пределами окна
                timestamps.RemoveAll(t => now - t > Window);

                if (timestamps.Count >= MaxRequests)
                {
                    _logger.LogWarning("Rate limit exceeded for IP {IP}", ip);
                    context.Response.StatusCode = 429;
                    context.Response.ContentType = "application/json";
                    context.Response.WriteAsync("{\"error\":\"Слишком много запросов. Попробуйте через минуту.\"}");
                    return;
                }

                timestamps.Add(now);
            }
        }

        await _next(context);
    }
}