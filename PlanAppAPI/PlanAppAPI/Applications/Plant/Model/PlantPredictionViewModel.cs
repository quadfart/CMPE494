namespace PlanAppAPI.Applications.Plant.Model;

public class PlantPredictionViewModel : Domain.Tables.Plant
{
    public float PredictionConfidence { get; set; }
}