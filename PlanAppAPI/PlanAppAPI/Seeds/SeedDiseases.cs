using Domain.Tables;
using Newtonsoft.Json;
using Persistence;

namespace PlanAppAPI.Seeds;

public class SeedDiseases
{
    public static async Task InitializeAsync(DataContext context)
    {
        if (File.Exists("Seeds/JsonData/Diseases.json"))
        {
            var seedDiseases = JsonConvert.DeserializeObject<List<Disease>>(File.ReadAllText("Seeds/JsonData/Diseases.json"));
            try
            {
                if (!context.Diseases.Any() && seedDiseases != null)
                {
                    var diseases = seedDiseases.Select(disease => new Disease
                        {
                            Name = disease.Name,
                            Symptoms = disease.Symptoms,
                            Treatments = disease.Treatments
                        })
                        .ToList();

                    await context.Diseases.AddRangeAsync(diseases);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred during seeding: {e.Message}");
                throw;
            }
        }
    }
}