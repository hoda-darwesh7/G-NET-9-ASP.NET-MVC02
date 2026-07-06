using Microsoft.EntityFrameworkCore;
using GymManagement.DAL.Configurations;
using GymManagement.DAL.Models;
using System.Numerics;
using GymManagement.DAL.Configrations;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GymManagement.DAL.Context
{
    public class GymDbContext : IdentityDbContext<ApplicationUser>
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MemberShip> MemberShips { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<HelthRecord> HelthRecords { get; set; }
    }
}
