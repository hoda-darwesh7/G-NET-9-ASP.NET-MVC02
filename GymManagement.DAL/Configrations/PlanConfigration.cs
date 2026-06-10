using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymManagement.Models;

namespace GymManagement.Configrations
{
    public class PlanConfigration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(x => x.Name).HasColumnType("varchar(30)");

            builder.Property(d => d.Description).HasMaxLength(200);

            builder.Property(p => p.Price).HasPrecision(10, 2);

            builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");

            builder.Property(U=>U.UpdatedAt).HasDefaultValueSql("GETDATE()");

            builder.ToTable(TB =>
            {
                TB.HasCheckConstraint("PlanDurationCheck", "DurationDays Between 1 and 365");
            });
        }

    }
}
