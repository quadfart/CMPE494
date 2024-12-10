using Domain.Tables;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<SensorData> SensorData => Set<SensorData>();
    
    public DbSet<SensorDataLog> SensorDataLogs => Set<SensorDataLog>();
    
    public DbSet<Disease> Diseases => Set<Disease>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.Entity<User>()
            .ToTable("tblUsers")
            .HasKey(x => x.Id);
        
        _ = modelBuilder.Entity<Plant>()
            .ToTable("tblPlants")
            .HasKey(x => x.Id);
        
        _ = modelBuilder.Entity<SensorData>()
            .ToTable("tblSensorData")
            .HasKey(x => x.Id);
        
        _ = modelBuilder.Entity<SensorDataLog>()
            .ToTable("tblSensorLogs")
            .HasKey(x => x.Id);
        
        _ = modelBuilder.Entity<Disease>()
            .ToTable("tblDiseases")
            .HasKey(x => x.Id);
        
        _ = modelBuilder.Entity<User>()
            .HasMany(u => u.SensorData)
            .WithOne()
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        _ = modelBuilder.Entity<SensorData>()
            .HasOne(sd => sd.Plant)
            .WithMany()
            .HasForeignKey(sd => sd.PlantId)
            .OnDelete(DeleteBehavior.NoAction);

        _ = modelBuilder.Entity<Plant>()
            .HasOne<Disease>(p => p.Diseases)
            .WithMany()
            .HasForeignKey(p => p.DiseaseId)
            .OnDelete(DeleteBehavior.NoAction);

        _ = modelBuilder.Entity<SensorDataLog>()
            .HasOne<SensorData>(sdl => sdl.SensorData)
            .WithMany()
            .HasForeignKey(sd => sd.SensorDataId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}