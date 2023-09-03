using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Adding the connection string to the container
builder.Services.AddDbContext<StoreContext>(opt => {
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); //  Adding AutoMapper to the container

var app = builder.Build();

// Configure the HTTP request pipeline.
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
