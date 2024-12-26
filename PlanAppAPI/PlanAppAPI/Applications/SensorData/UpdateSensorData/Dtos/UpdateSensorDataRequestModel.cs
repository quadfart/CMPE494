
using System.ComponentModel.DataAnnotations;
using PlanAppAPI.Applications.SensorData.BaseDto;

namespace PlanAppAPI.Applications.SensorData.UpdateSensorData.Dtos;

public class UpdateSensorDataRequestModel
{
    [Required]
    public int Id { get; set; }
    public int? PlantId { get; set; }
    public int? DiseaseId { get; set; }
}