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
            var tran = await context.Database.BeginTransactionAsync();
            try
            {
                if (!context.Plants.Any() && seedDiseases != null)
                {
                    var diseases = seedDiseases.Select(disease => new Disease
                        {
                            Id = disease.Id,
                            Name = disease.Name,
                            Symptoms = disease.Symptoms,
                            Treatments = disease.Treatments,

                        })
                        .ToList();

                    await context.Diseases.AddRangeAsync(diseases);
                    await context.SaveChangesAsync();
                }
                await tran.CommitAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await tran.RollbackAsync();
                throw;
            }
        }
    }
}