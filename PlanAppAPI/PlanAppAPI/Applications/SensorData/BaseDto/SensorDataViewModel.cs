using Domain.Tables;

namespace PlanAppAPI.Applications.SensorData.BaseDto;

public class SensorDataViewModel
{
    public int Id { get; set; }

    public int Temperature { get; set; }

    public int Moisture { get; set; }

    public DateTime Timestamp { get; set; }

    public int? PlantId { get; set; }
    
    public int? UserId { get; set; }
    
    public Domain.Tables.Plant? Plant { get; set; }
    
    public User? User { get; set; }
}