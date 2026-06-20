using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymManagement.DAL.Context
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext , string seedFolderpath ,ILogger logger)
        {

            try
            {
                if (!await dbContext.Plans.AnyAsync())
                {
                    var Plans = LoadDataFromJsonFile<Plan>(seedFolderpath, "Plans.json");
                    if (Plans.Any())
                    {
                        dbContext.Plans.AddRange(Plans);
                        logger.LogInformation($"Plans Seeded With Count = {Plans.Count}");
                    }

                    if (dbContext.ChangeTracker.HasChanges())
                        await dbContext.SaveChangesAsync();
                    else
                        logger.LogInformation("Plan Already Seeded");
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation("Seeding Fail");
                throw; 
            }
        }

        public static List<T> LoadDataFromJsonFile<T>(string folderPath , string Filename)
        {
            var filepath = Path.Combine(folderPath , Filename);
            if (!File.Exists(filepath))
                throw new FileNotFoundException("File Data Not Found!");
            var Data = File.ReadAllText(filepath);

            var Options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<T>>(Data , Options) ?? [];
        }
    }
}
