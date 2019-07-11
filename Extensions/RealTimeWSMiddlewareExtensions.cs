
using Gambling.Middleware;
using Microsoft.AspNetCore.Builder;

public static class CustomMiddlewareExtensions
{
    public static IApplicationBuilder UseRealTimeWSMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RealTimeWSMiddleware>();
    }
}