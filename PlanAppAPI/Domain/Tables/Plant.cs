namespace Domain.Tables;

public class Plant
{
    public int Id { get; set; }
    public int ModTemp { get; set; }
    public string SoilType { get; set; }
    public string LightNeed { get; set; }
    public int HumidityLevel { get; set; }
    public int WateringFrequency { get; set; }
    public int IrrigationAmount { get; set; }
    public string ScientificName { get; set; }

}