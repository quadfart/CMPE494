using System.ComponentModel.DataAnnotations;

namespace PlanAppAPI.Applications.SensorData.BaseDto;

public class ISensorData
{
    [Required]
    public int Temperature { get; set; }

    [Required]
    public int Moisture { get; set; }
}