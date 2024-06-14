using SmartTruckApi.Api.ApiExtentions;
using SmartTruckApi.Infrastructure.InfrastructureExtentions;
using SmartTruckApi.Application.ApplicationExtentions;
using SmartTruckApi.Persistance.PersistanceExtentions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddJWTTokenAuthentication(builder.Configuration);
builder.Services.AddMapper();
builder.Services.AddSwaggerGen();
builder.Services.AddServices();
builder.Services.AddInfrastructure();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.AddGlobalUserMiddleware();
app.AddExceptionHandler();

app.MapControllers();

/*using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
var initializer = new DbInitialaizer(serviceProvider);
await initializer.InitialaizeDb(CancellationToken.None);*/

app.Run();
