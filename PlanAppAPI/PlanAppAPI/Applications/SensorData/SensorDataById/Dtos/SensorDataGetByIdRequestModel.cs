using System.ComponentModel.DataAnnotations;

namespace PlanAppAPI.Applications.SensorData.SensorDataById.Dtos;

public class SensorDataGetByIdRequestModel
{
    [Required]
    public int Id { get; set; }
}