using API.Extensions;
using API.Middleware;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddApplicationServices(builder.Configuration); //  Adding the ApplicationServices to the container

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>(); //  Using the ExceptionMiddleware to handle exceptions
app.UseStatusCodePagesWithReExecute("/errors/{0}"); //  Using the ErrorController to handle errors

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(); // Able to serve static files from wwwroot

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope(); //  Creating a scope for the lifetime of the application
var services = scope.ServiceProvider; //  Getting the services from the scope
var context = services.GetRequiredService<StoreContext>(); //  Getting the StoreContext from the services
var logger = services.GetRequiredService<ILogger<Program>>(); //  Getting the logger from the services

try
{
    await context.Database.MigrateAsync(); //  Migrating the database
    await StoreContextSeed.SeedAsync(context); //  Seeding the database
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during migration");
}

app.Run();
