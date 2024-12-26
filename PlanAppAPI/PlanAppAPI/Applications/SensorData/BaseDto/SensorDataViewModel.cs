using Domain.Tables;

namespace PlanAppAPI.Applications.SensorData.BaseDto;

public class SensorDataViewModel
{
    public int Id { get; set; }
    
    public string? SensorSerialNumber { get; set; }

    public int? PlantId { get; set; }
    
    public int? UserId { get; set; }
    
}