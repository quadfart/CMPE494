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

        modelBuilder.Entity<User>()
            .ToTable("tblUsers")
            .HasKey(x => x.Id);

        modelBuilder.Entity<SensorData>()
            .ToTable("tblSensorData")
            .HasKey(x => x.Id);

        modelBuilder.Entity<SensorData>()
            .HasOne(u => u.User) // SensorData to User relationship
            .WithMany(u => u.SensorData)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SensorData>()
            .HasOne(p => p.Plant) // SensorData to Plant relationship
            .WithMany()
            .HasForeignKey(p => p.PlantId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SensorDataLog>()
            .ToTable("tblSensorLogs")
            .HasKey(x => x.Id);

        modelBuilder.Entity<SensorDataLog>()
            .HasOne(sdl => sdl.SensorData)
            .WithMany(sd => sd.SensorDataLogs)
            .HasForeignKey(sdl => sdl.SensorDataId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Plant>()
            .ToTable("tblPlants")
            .HasKey(x => x.Id);

        modelBuilder.Entity<Plant>()
            .HasOne(p => p.Diseases)
            .WithMany()
            .HasForeignKey(p => p.DiseaseId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Disease>()
            .ToTable("tblDiseases")
            .HasKey(x => x.Id);
    }
}