using Newtonsoft.Json;

namespace PlanAppAPI.Applications.Plant.Model;

public class PredictionResponseModel
{
    [JsonProperty("predictions")]
    public Dictionary<string,string> PredictionResponse { get; set; }
}