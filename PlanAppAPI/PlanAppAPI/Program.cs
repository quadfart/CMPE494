using Microsoft.EntityFrameworkCore;
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

// Migrate the database and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<DataContext>();
        context.Database.Migrate(); // Apply migrations

        // Run each seed independently
        try {
            Console.WriteLine("Seeding users...");
            await SeedUser.InitializeAsync(context).ConfigureAwait(false);
            Console.WriteLine("Users seeded successfully.");
        } catch (Exception ex) {
            Console.WriteLine($"Error seeding users: {ex.Message}");
        }

        try {
            Console.WriteLine("Seeding diseases...");
            await SeedDiseases.InitializeAsync(context).ConfigureAwait(false);
            Console.WriteLine("Diseases seeded successfully.");
        } catch (Exception ex) {
            Console.WriteLine($"Error seeding diseases: {ex.Message}");
        }

        try {
            Console.WriteLine("Seeding plants...");
            await SeedPlants.InitializeAsync(context).ConfigureAwait(false);
            Console.WriteLine("Plants seeded successfully.");
        } catch (Exception ex) {
            Console.WriteLine($"Error seeding plants: {ex.Message}");
        }

        Console.WriteLine("Seeding completed.");
    } catch (Exception ex) {
        var logger = services.GetService<ILogger<Program>>();
        logger?.LogError(ex, "An error occurred during migration or seeding.");
    }
}

await app.RunAsync().ConfigureAwait(false);
