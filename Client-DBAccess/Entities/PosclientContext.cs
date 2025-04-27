using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Client_DBAccess.Entities;

public partial class PosclientContext : DbContext
{
    public PosclientContext()
    {
    }

    public PosclientContext(DbContextOptions<PosclientContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Attend> Attends { get; set; }

    public virtual DbSet<AttendDetail> AttendDetails { get; set; }

    public virtual DbSet<Attribute> Attributes { get; set; }

    public virtual DbSet<AttributeSet> AttributeSets { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<ImgSet> ImgSets { get; set; }

    public virtual DbSet<Incurred> Incurreds { get; set; }

    public virtual DbSet<LPaymentMethod> LPaymentMethods { get; set; }

    public virtual DbSet<LStatus> LStatuses { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderActivityLog> OrderActivityLogs { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<SubCategory> SubCategories { get; set; }

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
            entity.Property(e => e.BrandId).HasMaxLength(400);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Creator).HasMaxLength(400);
            entity.Property(e => e.LastestModifiedBy).HasMaxLength(400);
            entity.Property(e => e.LastestModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(600);
            entity.Property(e => e.RoleId).HasMaxLength(400);
            entity.Property(e => e.Username).HasMaxLength(400);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Attend>(entity =>
        {
            entity.ToTable("Attend");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.CreatedBy).HasMaxLength(400);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DateEnd).HasColumnType("datetime");
            entity.Property(e => e.DateStart).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Attends)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attend_Account");
        });

        modelBuilder.Entity<AttendDetail>(entity =>
        {
            entity.ToTable("AttendDetail");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AccId).HasMaxLength(400);
            entity.Property(e => e.AttendId).HasMaxLength(400);
            entity.Property(e => e.TimeEnd).HasColumnType("datetime");
            entity.Property(e => e.TimeStart).HasColumnType("datetime");

            entity.HasOne(d => d.Acc).WithMany(p => p.AttendDetails)
                .HasForeignKey(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttendDetail_Account");

            entity.HasOne(d => d.Attend).WithMany(p => p.AttendDetails)
                .HasForeignKey(d => d.AttendId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttendDetail_Attend");
        });

        modelBuilder.Entity<Attribute>(entity =>
        {
            entity.ToTable("Attribute");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AttributeSetId).HasMaxLength(400);
            entity.Property(e => e.CreatedBy).HasMaxLength(400);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(400);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.AttributeSet).WithMany(p => p.Attributes)
                .HasForeignKey(d => d.AttributeSetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attribute_AttributeSet1");
        });

        modelBuilder.Entity<AttributeSet>(entity =>
        {
            entity.ToTable("AttributeSet");

            entity.Property(e => e.Id).HasMaxLength(400);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.CreatedBy).HasMaxLength(400);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(400);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employee");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AccId).HasMaxLength(400);
            entity.Property(e => e.Address).HasMaxLength(400);
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Fullname).HasMaxLength(400);
            entity.Property(e => e.ImgSetId).HasMaxLength(400);
            entity.Property(e => e.Phone).HasMaxLength(15);

            entity.HasOne(d => d.Acc).WithMany(p => p.Employees)
                .HasForeignKey(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_Account");

            entity.HasOne(d => d.ImgSet).WithMany(p => p.Employees)
                .HasForeignKey(d => d.ImgSetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_ImgSet");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("Image");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.ImgSetId).HasMaxLength(400);

            entity.HasOne(d => d.ImgSet).WithMany(p => p.Images)
                .HasForeignKey(d => d.ImgSetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Image_ImgSet");
        });

        modelBuilder.Entity<ImgSet>(entity =>
        {
            entity.ToTable("ImgSet");

            entity.Property(e => e.Id).HasMaxLength(400);
        });

        modelBuilder.Entity<Incurred>(entity =>
        {
            entity.ToTable("Incurred");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ShiftId).HasMaxLength(400);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Shift).WithMany(p => p.Incurreds)
                .HasForeignKey(d => d.ShiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incurred_Shift");
        });

        modelBuilder.Entity<LPaymentMethod>(entity =>
        {
            entity.ToTable("L_PaymentMethod");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<LStatus>(entity =>
        {
            entity.ToTable("L_Status");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Admin");

            entity.ToTable("Manager");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AccId).HasMaxLength(400);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fullname).HasMaxLength(300);
            entity.Property(e => e.ImgSetId).HasMaxLength(400);
            entity.Property(e => e.Phone).HasMaxLength(15);

            entity.HasOne(d => d.Acc).WithMany(p => p.Managers)
                .HasForeignKey(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manager_Account");

            entity.HasOne(d => d.ImgSet).WithMany(p => p.Managers)
                .HasForeignKey(d => d.ImgSetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manager_ImgSet");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreatedBy).HasMaxLength(400);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(400);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.ReasonCancel).HasMaxLength(500);
            entity.Property(e => e.ShiftId).HasMaxLength(400);

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_L_Status");
        });

        modelBuilder.Entity<OrderActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_OrderActivatyLog");

            entity.ToTable("OrderActivityLog");

            entity.Property(e => e.CreatedBy).HasMaxLength(400);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LogActivated).HasMaxLength(500);
            entity.Property(e => e.OrderId).HasMaxLength(400);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderActivityLogs)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderActivatyLog_Order");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("OrderDetail");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AttributeId).HasMaxLength(400);
            entity.Property(e => e.CreatedBy).HasMaxLength(400);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(400);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.OrderId).HasMaxLength(400);
            entity.Property(e => e.ProductId).HasMaxLength(400);

            entity.HasOne(d => d.Attribute).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.AttributeId)
                .HasConstraintName("FK_OrderDetail_Attribute");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Product");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.CreatedBy).HasMaxLength(400);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(400);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.OrderId).HasMaxLength(400);

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_Order");

            entity.HasOne(d => d.PaymentMethodNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethod)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payment_L_PaymentMethod");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AttributeSetId).HasMaxLength(400);
            entity.Property(e => e.CreatedBy).HasMaxLength(400);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImgSetId).HasMaxLength(400);
            entity.Property(e => e.ModifiedBy).HasMaxLength(400);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.SubCategoryId).HasMaxLength(400);

            entity.HasOne(d => d.AttributeSet).WithMany(p => p.Products)
                .HasForeignKey(d => d.AttributeSetId)
                .HasConstraintName("FK_Product_AttributeSet");

            entity.HasOne(d => d.ImgSet).WithMany(p => p.Products)
                .HasForeignKey(d => d.ImgSetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_ImgSet");

            entity.HasOne(d => d.SubCategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_SubCategory");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.ToTable("Setting");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.Addrress).HasMaxLength(500);
            entity.Property(e => e.BrandId).HasMaxLength(400);
            entity.Property(e => e.CreatedBy).HasMaxLength(400);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Hotline)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedBy).HasMaxLength(400);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Wifi).HasMaxLength(100);
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.ToTable("Shift");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AccId).HasMaxLength(400);
            entity.Property(e => e.TimeEnd).HasColumnType("datetime");
            entity.Property(e => e.TimeStart).HasColumnType("datetime");

            entity.HasOne(d => d.Acc).WithMany(p => p.Shifts)
                .HasForeignKey(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Shift_Account");
        });

        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.ToTable("SubCategory");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.CategoryId).HasMaxLength(400);
            entity.Property(e => e.CreatedBy).HasMaxLength(400);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.ModifiedBy).HasMaxLength(400);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Category).WithMany(p => p.SubCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubCategory_Category");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.ToTable("Token");

            entity.Property(e => e.Id).HasMaxLength(400);
            entity.Property(e => e.AccId).HasMaxLength(400);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            entity.Property(e => e.Value).HasMaxLength(600);

            entity.HasOne(d => d.Acc).WithMany(p => p.Tokens)
                .HasForeignKey(d => d.AccId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Token_Account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
