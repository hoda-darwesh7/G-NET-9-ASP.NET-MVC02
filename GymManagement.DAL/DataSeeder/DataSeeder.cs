using System.Text.Json;
using GymManagement.DAL.Context;
using GymManagement.DAL.Models; 
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Data
{
    public static class DataSeeder
    {
        public static async Task SeedPlansAsync(GymDbContext context)
        {
            if (await context.Plans.AnyAsync()) return;

            var filePath = "plans.json";
            if (!File.Exists(filePath)) return;

            var jsonContent = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var plans = JsonSerializer.Deserialize<List<Plan>>(jsonContent, options);

            if (plans != null)
            {
                await context.Plans.AddRangeAsync(plans);
                await context.SaveChangesAsync();
            }
        }
    }
}