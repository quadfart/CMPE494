using Domain.Tables;
using Newtonsoft.Json;
using Persistence;

namespace PlanAppAPI.Seeds;

public class SeedUser
{
    public static async Task InitializeAsync(DataContext context)
    {
        var tran = await context.Database.BeginTransactionAsync();
            try
            {
                if (!context.Users.Any())
                {
                    var user = new User
                    {
                        Name = "Uygar",
                        Email = "test@test.com",
                        Password = "123123"
                    };
                    
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                    await tran.CommitAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await tran.RollbackAsync();
                throw;
            }
    }
}