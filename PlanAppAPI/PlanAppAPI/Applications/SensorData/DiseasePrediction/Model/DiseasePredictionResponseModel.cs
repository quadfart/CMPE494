using Newtonsoft.Json;

namespace PlanAppAPI.Applications.SensorData.DiseasePrediction.Model;

public class DiseasePredictionResponseModel
{
    [JsonProperty("predictions")]
    public Dictionary<string,string> DiseasePredictionResponse { get; set; }
}