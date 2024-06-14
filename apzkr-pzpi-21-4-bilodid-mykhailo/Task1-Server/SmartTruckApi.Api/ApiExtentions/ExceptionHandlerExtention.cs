using SmartTruckApi.Api.ApiExtentions.Middlewares;

namespace SmartTruckApi.Api.ApiExtentions;

public static class ExceptionHandlerExtention
{
    public static IApplicationBuilder AddExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandler>();
        return app;
    }
}
