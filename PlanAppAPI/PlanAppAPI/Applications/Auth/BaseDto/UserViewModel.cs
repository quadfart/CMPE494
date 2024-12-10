namespace PlanAppAPI.Applications.Auth.BaseDto;

public class UserViewModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public ICollection<Domain.Tables.SensorData> SensorData { get; set; }
}