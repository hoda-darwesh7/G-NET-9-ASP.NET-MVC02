using GymManagement.DAL;
using GymManagement.DAL.Context;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymManagement.PL
{
    public static class ProgramExtension
    {
        public static async Task MigrateAndSeedDataAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var Logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var Migrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (Migrations.Any())
            {
                await dbContext.Database.MigrateAsync();
            }

            var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            await GymDataSeeding.SeedAsync(dbContext, seedFolderPath, Logger);
        }
    }
}
