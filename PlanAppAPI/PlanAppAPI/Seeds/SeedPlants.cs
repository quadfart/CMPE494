using Domain.Tables;
using OfficeOpenXml;
using Persistence;

namespace PlanAppAPI.Seeds;

public class SeedPlants
{
    public static async Task InitializeAsync(DataContext context)
    {
        var seedDataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "files", "PlantDataNew.xlsx");

        Console.WriteLine($"Looking for PlantDataNew.xlsx at: {seedDataFilePath}");

        if (!File.Exists(seedDataFilePath))
        {
            Console.WriteLine("PlantDataNew.xlsx not found. Seeding skipped.");
            return;
        }

        try
        {
            if (!context.Plants.Any())
            {
                var plants = new List<Plant>();
                Console.WriteLine("Reading data from PlantData.xlsx...");

                using (var package = new ExcelPackage(new FileInfo(seedDataFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension?.Rows ?? 0;

                    Console.WriteLine($"Found {rowCount - 1} rows of plant data.");

                    for (var row = 2; row <= rowCount; row++)
                    {
                        plants.Add(new Plant
                        {
                            ModTemp = worksheet.Cells[row, 1].GetValue<int>(),
                            SoilType = worksheet.Cells[row, 2].GetValue<string>(),
                            LightNeed = worksheet.Cells[row, 3].GetValue<string>(),
                            HumidityLevel = worksheet.Cells[row, 4].GetValue<int>(),
                            WateringFrequency = worksheet.Cells[row, 5].GetValue<int>(),
                            IrrigationAmount = worksheet.Cells[row, 6].GetValue<int>(),
                            ScientificName = worksheet.Cells[row, 7].GetValue<string>()
                        });
                    }
                }

                Console.WriteLine($"Adding {plants.Count} plants to the database...");
                await context.Plants.AddRangeAsync(plants);
                await context.SaveChangesAsync(); // EF Core manages the transaction
                Console.WriteLine("Plants seeding completed successfully.");
            }
            else
            {
                Console.WriteLine("Plants already exist in the database. Skipping seeding.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred during Plant seeding: {e.Message}");
            throw; // Rethrow the exception to ensure it’s logged at a higher level
        }
    }
}
