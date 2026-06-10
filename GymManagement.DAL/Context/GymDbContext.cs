using Microsoft.EntityFrameworkCore;
using GymManagement.DAL.Configurations;
using GymManagement.DAL.Models;
using System.Numerics;

namespace GymManagement.DAL.Context
{
    public class GymDbContext:DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options ) : base( options ) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Plan>(new PlanConfigration());
        }
        public DbSet<Plan> Plans { get; set; }
    }
}
