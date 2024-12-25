using Domain.Tables;
using Newtonsoft.Json;
using Persistence;

namespace PlanAppAPI.Seeds;

public class SeedDiseases
{
    public static async Task InitializeAsync(DataContext context)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "files", "Diseases.json");

        Console.WriteLine($"Looking for Diseases.json at: {filePath}");

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Diseases.json not found. Seeding skipped.");
            return;
        }

        try
        {
            var seedDiseases = JsonConvert.DeserializeObject<List<Disease>>(File.ReadAllText(filePath));
            if (!context.Diseases.Any() && seedDiseases != null)
            {
                var diseases = seedDiseases.Select(disease => new Disease
                    {
                        Name = disease.Name,
                        Symptoms = disease.Symptoms,
                        Treatments = disease.Treatments
                    })
                    .ToList();

                Console.WriteLine($"Adding {diseases.Count} diseases to the database...");
                await context.Diseases.AddRangeAsync(diseases);
                await context.SaveChangesAsync(); // EF Core manages the transaction
                Console.WriteLine("Diseases seeding completed.");
            }
            else
            {
                Console.WriteLine("Diseases already exist in the database. Skipping seeding.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred during seeding: {e.Message}");
            throw; // Rethrow the exception to ensure it’s logged at a higher level
        }
    }
}