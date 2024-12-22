namespace PlanAppAPI.Applications.SensorDataLog.AddSensorDataLog.Dtos
{
    public class AddSensorDataLogRequestModel
    {
        public string SensorSerialNumber { get; set; } = default!;
        public int Temperature { get; set; }
        public int Moisture { get; set; }
        public int SoilMoisture { get; set; }
    }
}