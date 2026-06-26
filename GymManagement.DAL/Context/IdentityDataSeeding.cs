using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Context
{
    public class IdentityDataSeeding
    {
        public static async Task SeedIdentityData(RoleManager<IdentityRole> roleManager ,
                                            UserManager<ApplicationUser> userManager ,
                                            ILogger Logger ,
                                            CancellationToken ct = default)
        {
            try
            {
                bool HasUsers = await userManager.Users.AnyAsync(ct);
                bool HasRoles = await roleManager.Roles.AnyAsync(ct);
                if (HasUsers && HasRoles) return;
                var roles = new List<IdentityRole>()
            {
                new IdentityRole("SuperAdmin"),
                new IdentityRole("Admin")
            };

                foreach (var role in roles)
                {
                    if (await roleManager.RoleExistsAsync(role.Name))
                    {
                        var roleResult = await roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            Logger.LogError($"Failed to Add Role {role.Name}");
                        }
                        ;
                    }
                }

                if (!HasUsers)
                {
                    var mainAdmin = new ApplicationUser()
                    {
                        FirstName = "Mahmoud",
                        LastName = "Darwesh",
                        Email = "darwesh@gmail.com",
                        UserName = "Mahmoud Darwesh",
                        PhoneNumber = "01128025717"

                    };

                    await userManager.CreateAsync(mainAdmin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(mainAdmin, "SuperAdmin");

                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Aya",
                        LastName = "Darwesh",
                        Email = "aya@gmail.com",
                        UserName = "Aya Darwesh",
                        PhoneNumber = "01128025718"

                    };

                    await userManager.CreateAsync(Admin, "P@SSw0rd");
                    await userManager.AddToRoleAsync(Admin, "Admin");

                    Logger.LogInformation("Identity Seeded Successfully");
                }

            }

            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return;
            }

        }
    }
}
