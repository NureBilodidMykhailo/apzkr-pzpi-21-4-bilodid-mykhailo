using SmartTruckApi.Api.ApiExtentions.Middlewares;

namespace SmartTruckApi.Api.ApiExtentions;

public static class GlobalUserExtention
{
    public static IApplicationBuilder AddGlobalUserMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalUserMiddleware>();
        return app;
    }
}