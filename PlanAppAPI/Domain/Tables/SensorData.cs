namespace Domain.Tables;

public class SensorData
{
    public int Id { get; set; }
    public int Status { get; set; } // 1 = Active, 0 = Inactive

    public string? SensorSerialNumber { get; set; } // Nullable, if no sensor is assigned yet
    public int? UserId { get; set; } // Foreign key to User
    public User? User { get; set; } // Navigation property for EF

    public int? PlantId { get; set; } // Nullable, plant assigned to the pot
    public Plant? Plant { get; set; } // Navigation property for EF

    public ICollection<SensorDataLog>? SensorDataLogs { get; set; } // Zero or many logs
}