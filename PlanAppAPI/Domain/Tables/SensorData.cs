namespace Domain.Tables;

public class SensorData
{
    public int Id { get; set; }

    public int Status { get; set; } // 1 = Active, 0 = Inactive

    public int? PlantId { get; set; }
    
    public int? UserId { get; set; }
    
    public Plant? Plant { get; set; }
}