using Persistence;
using PlanAppAPI.Extensions;
using PlanAppAPI.Seeds;

var builder = WebApplication.CreateBuilder(args);

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
builder.Configuration.AddJsonFile($"appsettings.{environmentName}.json", true, false);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlantAppAPI V1");
    });
}

app.UseRouting();
app.MapControllers();

// Migrate the database with seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<DataContext>();
        await SeedUser.InitializeAsync(context).ConfigureAwait(false);
        // await SeedDiseases.InitializeAsync(context).ConfigureAwait(false);
        await SeedPlants.InitializeAsync(context).ConfigureAwait(false);
    } catch (Exception ex) {
        var logger = services.GetService<ILogger<Program>>();
        logger?.LogError(ex, "An error occured during Migration");
    }   
}

await app.RunAsync().ConfigureAwait(false);