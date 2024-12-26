namespace PlanAppAPI.Applications.SensorData.UpdateSensorData.Dtos;

public class UpdateSensorDataResponseModel
{
    public int Id { get; set; }
    
    public string? SensorSerialNumber { get; set; }

    public int? PlantId { get; set; }
    
    public int? UserId { get; set; }
    
}