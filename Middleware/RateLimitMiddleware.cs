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

    // Чистим словарь каждые 10 минут
    private static DateTime _lastCleanup = DateTime.UtcNow;
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromMinutes(10);

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

            // Периодическая очистка устаревших IP
            if (now - _lastCleanup > CleanupInterval)
            {
                _lastCleanup = now;
                CleanupStore(now);
            }

            var timestamps = _store.GetOrAdd(ip, _ => new List<DateTime>());

            lock (timestamps)
            {
                // Удаляем записи за пределами окна
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

    private static void CleanupStore(DateTime now)
    {
        // Собираем IP у которых все записи устарели
        var staleKeys = new List<string>();

        foreach (var kv in _store)
        {
            lock (kv.Value)
            {
                kv.Value.RemoveAll(t => now - t > Window);
                if (kv.Value.Count == 0)
                    staleKeys.Add(kv.Key);
            }
        }

        foreach (var key in staleKeys)
            _store.TryRemove(key, out _);
    }
}