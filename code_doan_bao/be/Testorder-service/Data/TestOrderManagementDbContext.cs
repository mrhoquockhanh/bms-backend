using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Testorder_service.Models.Entities;

namespace Testorder_service.Data;

public partial class TestOrderManagementDbContext : DbContext
{
    public TestOrderManagementDbContext()
    {
    }

    public TestOrderManagementDbContext(DbContextOptions<TestOrderManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<BloodSample> BloodSamples { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorRoom> DoctorRooms { get; set; }

    public virtual DbSet<DoctorShift> DoctorShifts { get; set; }

    public virtual DbSet<LabAssignment> LabAssignments { get; set; }

    public virtual DbSet<LabRoom> LabRooms { get; set; }

    public virtual DbSet<LabUserProfile> LabUserProfiles { get; set; }

    public virtual DbSet<TestCriterion> TestCriteria { get; set; }

    public virtual DbSet<TestOrderComment> TestOrderComments { get; set; }

    public virtual DbSet<TestPackage> TestPackages { get; set; }

    public virtual DbSet<TestPackageCriterion> TestPackageCriteria { get; set; }

    public virtual DbSet<TestParameter> TestParameters { get; set; }

    public virtual DbSet<TestResult> TestResults { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>().HasKey(e => e.AppointmentId);
        modelBuilder.Entity<Appointment>().Property(e => e.AppointmentId).HasElementName("_id");
        modelBuilder.Entity<BloodSample>().HasKey(e => e.SampleId);
        modelBuilder.Entity<BloodSample>().Property(e => e.SampleId).HasElementName("_id");
        modelBuilder.Entity<Doctor>().HasKey(e => e.DoctorId);
        modelBuilder.Entity<Doctor>().Property(e => e.DoctorId).HasElementName("_id");
        modelBuilder.Entity<DoctorRoom>().HasKey(e => e.RoomId);
        modelBuilder.Entity<DoctorRoom>().Property(e => e.RoomId).HasElementName("_id");
        modelBuilder.Entity<DoctorShift>().HasKey(e => e.ShiftId);
        modelBuilder.Entity<DoctorShift>().Property(e => e.ShiftId).HasElementName("_id");
        modelBuilder.Entity<LabAssignment>().HasKey(e => e.AssignmentId);
        modelBuilder.Entity<LabAssignment>().Property(e => e.AssignmentId).HasElementName("_id");
        modelBuilder.Entity<LabRoom>().HasKey(e => e.RoomId);
        modelBuilder.Entity<LabRoom>().Property(e => e.RoomId).HasElementName("_id");
        modelBuilder.Entity<LabUserProfile>().HasKey(e => e.UserId);
        modelBuilder.Entity<LabUserProfile>().Property(e => e.UserId).HasElementName("_id");
        modelBuilder.Entity<TestCriterion>().HasKey(e => e.CriteriaId);
        modelBuilder.Entity<TestCriterion>().Property(e => e.CriteriaId).HasElementName("_id");
        modelBuilder.Entity<TestOrderComment>().HasKey(e => e.CommentId);
        modelBuilder.Entity<TestOrderComment>().Property(e => e.CommentId).HasElementName("_id");
        modelBuilder.Entity<TestPackage>().HasKey(e => e.PackageId);
        modelBuilder.Entity<TestPackage>().Property(e => e.PackageId).HasElementName("_id");
        modelBuilder.Entity<TestPackageCriterion>().HasKey(e => e.TpcId);
        modelBuilder.Entity<TestPackageCriterion>().Property(e => e.TpcId).HasElementName("_id");
        modelBuilder.Entity<TestParameter>().HasKey(e => e.ParamId);
        modelBuilder.Entity<TestParameter>().Property(e => e.ParamId).HasElementName("_id");
        modelBuilder.Entity<TestResult>().HasKey(e => e.TestResultId);
        modelBuilder.Entity<TestResult>().Property(e => e.TestResultId).HasElementName("_id");


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
