using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence;

namespace PlanAppAPI.Extensions;

public static class ApplicationServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        _ = services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlantAppAPI", Version = "v1" });
        });
        
        _ = services.AddDbContext<DataContext>(opt =>
        {
            _ = opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        });

        _ = services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
        });
        
        return services;
    }
}