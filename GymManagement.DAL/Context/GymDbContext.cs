using Microsoft.EntityFrameworkCore;
using GymManagement.Configrations;
using GymManagement.Models;
using System.Numerics;

namespace GymManagement.Context
{
    public class GymDbContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = 7ODA ; Database = GymDb2 ; Trusted_Connection = true ; TrustServerCertificate = true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Plan>(new PlanConfigration());
        }
        public DbSet<Plan> Plans { get; set; }
    }
}
