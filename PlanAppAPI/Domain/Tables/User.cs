namespace Domain.Tables;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public ICollection<SensorData>? SensorData { get; set; }
}