using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Patient_service.Models;

namespace Patient_service;

public partial class PatientServiceContext : DbContext
{
    public PatientServiceContext()
    {
    }

    public PatientServiceContext(DbContextOptions<PatientServiceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LabCriterion> LabCriteria { get; set; }

    public virtual DbSet<LabTest> LabTests { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LabCriterion>(entity =>
        {
            entity.HasKey(e => e.Id);




            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .IsUnicode(false)
                ;
            entity.Property(e => e.CreatedAt)
                
                ;
            entity.Property(e => e.CriteriaName)
                .HasMaxLength(200)
                .IsUnicode(false)
                ;
            entity.Property(e => e.ReferenceRange)
                .HasMaxLength(100)
                .IsUnicode(false)
                ;
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .IsUnicode(false)
                ;
        });

        modelBuilder.Entity<LabTest>(entity =>
        {
            entity.HasKey(e => e.Id);




            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .IsUnicode(false)
                ;
            entity.Property(e => e.CreatedAt)
                
                ;
            entity.Property(e => e.Description)
                
                ;
            entity.Property(e => e.Price)
                
                ;
            entity.Property(e => e.TestName)
                .HasMaxLength(200)
                .IsUnicode(false)
                ;

            entity.HasMany(d => d.Criteria).WithMany(p => p.LabTests)
                .UsingEntity<Dictionary<string, object>>(
                    "LabTestCriterion",
                    r => r.HasOne<LabCriterion>().WithMany()
                        .HasForeignKey("CriteriaId")
                        ,
                    l => l.HasOne<LabTest>().WithMany()
                        .HasForeignKey("LabTestId")
                        ,
                    j =>
                    {
                        j.HasKey("LabTestId", "CriteriaId");


                        j.IndexerProperty<string>("LabTestId")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            ;
                        j.IndexerProperty<string>("CriteriaId")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            ;
                    });
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.Id);




            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .IsUnicode(false)
                ;
            entity.Property(e => e.CreatedAt)
                
                ;
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                ;
            entity.Property(e => e.Description)
                
                ;
            
            
            entity.Property(e => e.PatientId)
                .HasMaxLength(20)
                .IsUnicode(false)
                ;
            
            entity.Property(e => e.RecordType)
                .HasMaxLength(50)
                .IsUnicode(false)
                ;
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                
                ;
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false)
                ;
            entity.Property(e => e.UpdatedAt)
                
                ;
            entity.Property(e => e.VerifiedBy)
                .HasMaxLength(20)
                .IsUnicode(false)
                ;

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalRecords)
                .HasForeignKey(d => d.PatientId)
                ;

            entity.HasMany(d => d.LabTests).WithMany(p => p.MedicalRecords)
                .UsingEntity<Dictionary<string, object>>(
                    "MedicalRecordLabTest",
                    r => r.HasOne<LabTest>().WithMany()
                        .HasForeignKey("LabTestId")
                        ,
                    l => l.HasOne<MedicalRecord>().WithMany()
                        .HasForeignKey("MedicalRecordId")
                        ,
                    j =>
                    {
                        j.HasKey("MedicalRecordId", "LabTestId");


                        j.IndexerProperty<string>("MedicalRecordId")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            ;
                        j.IndexerProperty<string>("LabTestId")
                            .HasMaxLength(20)
                            .IsUnicode(false)
                            ;
                    });
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Id);





            entity.HasIndex(e => e.Fullname, "idx_patients_name");

            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .IsUnicode(false)
                ;
            entity.Property(e => e.Address)
   // Unicode
.IsUnicode(true)
;
            entity.Property(e => e.BloodType)
                .HasMaxLength(10)
                .IsUnicode(false)
                ;
            entity.Property(e => e.CreatedAt)
                
                ;
            
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                ;
            entity.Property(e => e.Fullname)
                .HasMaxLength(200)
                .IsUnicode(false)
                ;
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false)
                ;
            entity.Property(e => e.IdentityNumber)
                .HasMaxLength(100)
                .IsUnicode(false)
                ;
            
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false)
                ;
            entity.Property(e => e.UpdatedAt)
                
                ;
        });

    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
