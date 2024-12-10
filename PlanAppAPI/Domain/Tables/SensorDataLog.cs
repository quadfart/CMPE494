namespace Domain.Tables;

public class SensorDataLog
{
    public int Id { get; set; }
    
    public int Temperature { get; set; }

    public int Moisture { get; set; }

    public DateTime Timestamp { get; set; }

    public int SensorDataId { get; set; }

    public SensorData SensorData { get; set; }
}