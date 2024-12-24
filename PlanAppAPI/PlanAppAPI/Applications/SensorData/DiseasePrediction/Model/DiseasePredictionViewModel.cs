namespace PlanAppAPI.Applications.SensorData.DiseasePrediction.Model;

public class DiseasePredictionViewModel : Domain.Tables.Disease
{
    public float DiseasePredictionConfidence { get; set; }
}