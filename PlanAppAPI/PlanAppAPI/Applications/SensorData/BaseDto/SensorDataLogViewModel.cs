namespace PlanAppAPI.Applications.SensorData.BaseDto;

public class SensorDataLogViewModel
{
    public int Id { get; set; }

    public int Temperature { get; set; }

    public int Moisture { get; set; }

    public DateTime Timestamp { get; set; }
    public int SoilMoisture { get; set; }
}