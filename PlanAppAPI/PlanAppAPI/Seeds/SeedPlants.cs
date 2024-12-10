using Domain.Tables;
using Newtonsoft.Json;
using Persistence;

namespace PlanAppAPI.Seeds;

public class SeedPlants
{
    public static async Task InitializeAsync(DataContext context)
    {
        if (File.Exists("Seeds/JsonData/Plants.json"))
        {
            var seedPlants = JsonConvert.DeserializeObject<List<Plant>>(File.ReadAllText("Seeds/JsonData/Plants.json"));
            var tran = await context.Database.BeginTransactionAsync();
            try
            {
                if (!context.Plants.Any() && seedPlants != null)
                {
                    var plants = seedPlants.Select(plant => new Plant
                        {
                            Id = plant.Id,
                            ModTemp = plant.ModTemp,
                            SoilType = plant.SoilType,
                            LightNeed = plant.LightNeed,
                            HumidityLevel = plant.HumidityLevel,
                            WateringFrequency = plant.WateringFrequency,
                            IrrigationAmount = plant.IrrigationAmount,
                            ScientificName = plant.ScientificName,
                            DiseaseId = plant.DiseaseId,
                        })
                        .ToList();

                    await context.Plants.AddRangeAsync(plants);
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