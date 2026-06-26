using GymManagement.DAL.Repositories.Interfaces;
using GymManagement.DAL.Repositories;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Context;
using Microsoft.EntityFrameworkCore;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Profiles;
using GymManagement.PL;
using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace GymManagement.DAL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(X => X.AddProfile(new MappingProfile()));
            builder.Services.AddScoped(typeof(IGenericRepository<>) , typeof(GenericRepository<>));
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                //config.Password.RequireUppercase = true;
                //config.Password.RequireLowercase = true;

                config.User.RequireUniqueEmail = true;
                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                config.Lockout.MaxFailedAccessAttempts = 5;
            }).AddEntityFrameworkStores<GymDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {

            });

            builder.Services.AddDbContext<GymDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            var app = builder.Build();

            // SEEDING
            await app.MigrateAndSeedDataAsync();

            


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

           

            app.Run();


        }
    }
}
