using Domain.Tables;
using Newtonsoft.Json;
using Persistence;

namespace PlanAppAPI.Seeds;

public class SeedUser
{
    public static async Task InitializeAsync(DataContext context)
    {
        try
        {
            // Check if users already exist to avoid duplicates
            if (!context.Users.Any())
            {
                var user = new User
                {
                    Name = "Uygar",
                    Email = "test@test.com",
                    Password = "123123"
                };

                context.Users.Add(user);

                // Save changes, EF Core automatically handles transactions
                await context.SaveChangesAsync();
                Console.WriteLine("User seeded successfully.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error seeding user: {e.Message}");
            throw; // Re-throw the exception to ensure it's logged at a higher level
        }
    }
}