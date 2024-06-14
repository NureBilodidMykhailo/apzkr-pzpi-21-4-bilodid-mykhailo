using MongoDB.Bson;
using SmartTruckApi.Application.GlobalInstances;
using System.Security.Claims;

namespace SmartTruckApi.Api.ApiExtentions.Middlewares;

public class GlobalUserMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalUserMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (ObjectId.TryParse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out ObjectId id))
        {
            GlobalUser.Id = id;
        }
        GlobalUser.Email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        GlobalUser.Phone = httpContext.User.FindFirst(ClaimTypes.MobilePhone)?.Value;
        foreach (var role in httpContext.User.FindAll(ClaimTypes.Role))
        {
            GlobalUser.Roles.Add(role.Value);
        }
        await this._next(httpContext);
    }
}
