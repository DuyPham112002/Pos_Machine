using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Cosplane_API_DBAccess.Entities;

public partial class PosmanagementContext : DbContext
{
    public PosmanagementContext()
    {
    }

    public PosmanagementContext(DbContextOptions<PosmanagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("need connection string here");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Account");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Creator).HasMaxLength(400);
            entity.Property(e => e.LastestModifiedBy).HasMaxLength(400);
            entity.Property(e => e.LastestModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.RoleId).HasMaxLength(400);
            entity.Property(e => e.Username).HasMaxLength(400);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("Brand");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.Creator).HasMaxLength(400);
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.CreatorNavigation).WithMany(p => p.Brands)
                .HasForeignKey(d => d.Creator)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Brand_Account");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("Device");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.BrandId).HasMaxLength(400);
            entity.Property(e => e.CurrentAccount).HasMaxLength(400);
            entity.Property(e => e.DeviceFingerPrint).HasMaxLength(400);

            entity.HasOne(d => d.Brand).WithMany(p => p.Devices)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Device_Brand");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AccId).HasMaxLength(400);
            entity.Property(e => e.Address).HasMaxLength(600);
            entity.Property(e => e.Fullname).HasMaxLength(400);
            entity.Property(e => e.Mail).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(13);

            entity.HasOne(d => d.Acc).WithMany(p => p.Employees)
                .HasForeignKey(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Account");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.ToTable("Token");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AccId).HasMaxLength(400);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Value).HasMaxLength(500);

            entity.HasOne(d => d.Acc).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Token_Account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
