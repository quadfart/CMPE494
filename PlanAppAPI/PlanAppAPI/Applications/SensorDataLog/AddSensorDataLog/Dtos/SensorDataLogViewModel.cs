namespace PlanAppAPI.Applications.SensorDataLog.AddSensorDataLog.Dtos
{
    public class SensorDataLogViewModel
    {
        public int Id { get; set; }
        public string SensorSerialNumber { get; set; } = default!;
        public int Temperature { get; set; }
        public int Moisture { get; set; }
        public int SoilMoisture { get; set; }
        public DateTime Timestamp { get; set; }
    }
}