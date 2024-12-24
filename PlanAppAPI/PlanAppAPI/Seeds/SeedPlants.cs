using Domain.Tables;
using OfficeOpenXml;
using Persistence;

namespace PlanAppAPI.Seeds;

public class SeedPlants
{
    public static async Task InitializeAsync(DataContext context)
    {
        var seedDataFilePath = "app/files/PlantData.xlsx";

        if (File.Exists(seedDataFilePath))
        {
            using var transaction =
                context.Database.CurrentTransaction ?? await context.Database.BeginTransactionAsync();
            try
            {
                if (!context.Plants.Any())
                {
                    var plants = new List<Plant>();
                    using (var package = new ExcelPackage(new FileInfo(seedDataFilePath)))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

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

                    await context.Plants.AddRangeAsync(plants);
                    await context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                Console.WriteLine("Plants seeded successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}