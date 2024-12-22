namespace Domain.Tables;

public class Disease
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<string> Symptoms { get; set; }
    public List<string> Treatments { get; set; }
}