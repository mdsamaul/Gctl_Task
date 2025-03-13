using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GtclTaskProject.Models;

public partial class GctlinfoExamTest2025Context : DbContext
{
    public GctlinfoExamTest2025Context()
    {
    }

    public GctlinfoExamTest2025Context(DbContextOptions<GctlinfoExamTest2025Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerType> CustomerTypes { get; set; }

    public virtual DbSet<DeliveryAddress> DeliveryAddresses { get; set; }

    public virtual DbSet<HrmAtdRosterScheduleEntry> HrmAtdRosterScheduleEntries { get; set; }

    public virtual DbSet<HrmAtdShift> HrmAtdShifts { get; set; }

    public virtual DbSet<HrmDefDesignation> HrmDefDesignations { get; set; }

    public virtual DbSet<HrmEmployee> HrmEmployees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=S_SAMAUL;Initial Catalog=GCTLInfo_Exam_Test_2025;User id=sa;Password=12345; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CreditLimit).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<CustomerType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_CustomerId");

            entity.ToTable("CustomerType");

            entity.Property(e => e.CustomerType1)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("CustomerType");
        });

        modelBuilder.Entity<DeliveryAddress>(entity =>
        {
            entity.HasIndex(e => e.CustomerId, "IX_DeliveryAddresses_CustomerId");

            entity.HasOne(d => d.Customer).WithMany(p => p.DeliveryAddresses).HasForeignKey(d => d.CustomerId);
        });

        modelBuilder.Entity<HrmAtdRosterScheduleEntry>(entity =>
        {
            entity.HasKey(e => e.AiId).HasName("PK_HrmAtdRosterScheduleId");

            entity.ToTable("HRM_ATD_RosterScheduleEntry");

            entity.Property(e => e.AiId)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("AI_ID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .HasColumnName("EmployeeID");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.RosterScheduleCode).HasMaxLength(50);
        });

        modelBuilder.Entity<HrmAtdShift>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HRM_ATD_Shift");

            entity.Property(e => e.AbsentTime).HasColumnType("datetime");
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.LateTime).HasColumnType("datetime");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.ShiftCode).ValueGeneratedOnAdd();
            entity.Property(e => e.ShiftEndTime).HasColumnType("datetime");
            entity.Property(e => e.ShiftName).HasMaxLength(50);
            entity.Property(e => e.ShiftShortName).HasMaxLength(250);
            entity.Property(e => e.ShiftStartTime).HasColumnType("datetime");
            entity.Property(e => e.Wef)
                .HasColumnType("datetime")
                .HasColumnName("WEF");
        });

        modelBuilder.Entity<HrmDefDesignation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HRM_Def_Designation");

            entity.Property(e => e.AiId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AI_ID");
            entity.Property(e => e.DesignationCode).HasMaxLength(50);
            entity.Property(e => e.DesignationName).HasMaxLength(100);
            entity.Property(e => e.DesignationShortName).HasMaxLength(50);
            entity.Property(e => e.EntryDate).HasColumnType("datetime");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<HrmEmployee>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("HRM_Employee");

            entity.Property(e => e.AiId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AI_ID");
            entity.Property(e => e.DesignationCode).HasMaxLength(50);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .HasColumnName("EmployeeID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
