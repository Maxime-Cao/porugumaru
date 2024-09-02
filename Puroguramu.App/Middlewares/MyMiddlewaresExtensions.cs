namespace Puroguramu.App.Middlewares;

public static class MyMiddlewareExtensions
{
    public static IApplicationBuilder UseReverseProxyLinks(this IApplicationBuilder app)
        => app.UseMiddleware<ReverseProxyLinksMiddleware>();
}
