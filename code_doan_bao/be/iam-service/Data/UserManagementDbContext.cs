using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using iam_service.Modals;

namespace iam_service.Data;

public partial class UserManagementDBContext : DbContext
{
    public UserManagementDBContext(DbContextOptions<UserManagementDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Account { get; set; }

    public virtual DbSet<OtpCodes> OtpCodes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasElementName("_id");

            entity.HasIndex(e => e.Username, "Account_UserName_key").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                
                ;
            entity.Property(e => e.Email).HasMaxLength(100);
            
            entity.Property(e => e.Password).HasMaxLength(255);
            
            entity.Property(e => e.Username).HasMaxLength(100);

            // Removed relational configuration as MongoDB EF Core does not support Foreign Keys
        });

        modelBuilder.Entity<OtpCodes>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasElementName("_id");

            entity.Property(e => e.CreatedAt)
                
                ;
            entity.Property(e => e.Email).HasMaxLength(256);
            
            entity.Property(e => e.OtpCode).HasMaxLength(10);
            
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);
            entity.Property(e => e.RoleId).HasElementName("_id");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
