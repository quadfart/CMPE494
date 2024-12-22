using System.ComponentModel.DataAnnotations;
using PlanAppAPI.Applications.SensorData.BaseDto;

namespace PlanAppAPI.Applications.SensorData.AddSensorData.Dtos;

public class AddSensorDataRequestModel : ISensorData
{
    [Required]
    public int UserId { get; set; } // User creating the pot (sensor data)

    public string? SensorSerialNumber { get; set; } // Optional, nullable
}