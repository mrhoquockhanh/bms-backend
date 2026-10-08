using Microsoft.EntityFrameworkCore;
using warehouse_service.Models;
using warehouse_service.Models.Entities;

namespace warehouse_service.Data
{
    public class WarehouseDbContext : DbContext
    {
        public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
        {
        }

        public DbSet<Equipment>     Equipment { get; set; } = null!;
        public DbSet<Reagent> Reagents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Equipment>(entity =>
            {


                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(20);
                entity.Property(e => e.Manufacturer).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50);
                
                entity.Property(e => e.EquipmentName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(255);
            });

            modelBuilder.Entity<Reagent>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).HasMaxLength(20);
                entity.Property(r => r.ReagentCode).IsRequired().HasMaxLength(50);
                entity.HasIndex(r => r.ReagentCode).IsUnique();
                entity.Property(r => r.ReagentName).IsRequired().HasMaxLength(100);
                entity.Property(r => r.Manufacturer).HasMaxLength(100);
                entity.Property(r => r.Quantity).IsRequired();
                entity.Property(r => r.ImportDate);
                entity.Property(r => r.ExpiryDate);
                entity.Property(r => r.Status).HasMaxLength(100);
                entity.Property(r => r.Description).HasMaxLength(255);
                entity.Property(r => r.ImportLot).HasMaxLength(50);
                entity.Property(r => r.Unit).HasMaxLength(20);


            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
