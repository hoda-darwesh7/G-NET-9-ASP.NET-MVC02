using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Configrations
{
    public class GymUserConfiguration<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(u => u.Name).HasColumnType("varchar").HasMaxLength(50).IsRequired();

            builder.Property(u => u.Email).HasColumnType("varchar").HasMaxLength(100).IsRequired();
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Phone).HasMaxLength(11).IsRequired();
            builder.HasIndex(u => u.Phone).IsUnique();

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("EmailCheck", "Email Like '_%@_%._%'");
            });
            // Egyptian phone CHECK constraint per spec
            builder.ToTable(t => t.HasCheckConstraint(
                "PhoneCheck", "Phone Like '010%' OR Phone Like '011%' " +
                "OR Phone Like '012%' OR Phone Like '015%'"));

            // Owned Address — both Street & City varchar(30)
            builder.OwnsOne(u => u.Address, addr =>
            {
                addr.Property(a => a.Street)
                .HasColumnName("Street")
                .HasColumnType("varchar")
                .HasMaxLength(30);
                addr.Property(a => a.City)
                .HasColumnName("City")
                .HasColumnType("varchar")
                .HasMaxLength(30);
            });
        } 
    }
    
    
}
