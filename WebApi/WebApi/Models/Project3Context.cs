using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebApi.Models
{
    public partial class Project3Context : DbContext
    {
        public Project3Context()
        {
        }

        public Project3Context(DbContextOptions<Project3Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<EmpOfCustomer> EmpOfCustomers { get; set; }
        public virtual DbSet<RequestCustomer> RequestCustomers { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
        public virtual DbSet<ServiceCustomer> ServiceCustomers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Name=ConnectionStrings:DBConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.EmployeeId)
                    .HasName("PK_Users");

                entity.ToTable("Account");

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.ConfirmToken)
                    .HasMaxLength(10)
                    .HasColumnName("confirmToken")
                    .IsFixedLength(true);

                entity.Property(e => e.Department)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("image");

                entity.Property(e => e.PasswordHash).IsRequired();

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasDefaultValueSql("((2))");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.Department)
                    .HasConstraintName("FK_Users_Departments1");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Roles");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(155)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PasswordHash).IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);
            });

            modelBuilder.Entity<EmpOfCustomer>(entity =>
            {
                entity.ToTable("EmpOfCustomer");

                entity.Property(e => e.EmpId)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.ServiceOfCusNavigation)
                    .WithMany(p => p.EmpOfCustomers)
                    .HasForeignKey(d => d.ServiceOfCus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmpOfCustomer_Account");

                entity.HasOne(d => d.ServiceOfCus1)
                    .WithMany(p => p.EmpOfCustomers)
                    .HasForeignKey(d => d.ServiceOfCus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmpOfCustomer_ServiceCustomer");
            });

            modelBuilder.Entity<RequestCustomer>(entity =>
            {
                entity.ToTable("RequestCustomer");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.FullName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Message).HasColumnType("text");

                entity.Property(e => e.ServiceId)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Subject)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.RequestCustomers)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_RequestCustomer_Services");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Role1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Role");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("image");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ServiceCategoryId).HasColumnName("ServiceCategoryID");

                entity.HasOne(d => d.ServiceCategory)
                    .WithMany(p => p.Services)
                    .HasForeignKey(d => d.ServiceCategoryId)
                    .HasConstraintName("FK_Services_ServiceCategory");
            });

            modelBuilder.Entity<ServiceCategory>(entity =>
            {
                entity.ToTable("ServiceCategory");

                entity.Property(e => e.CaterogoryName)
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ServiceCustomer>(entity =>
            {
                entity.ToTable("ServiceCustomer");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.ServiceId)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.ServiceCustomers)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_ServiceCustomer_Customers");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ServiceCustomers)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServiceCustomer_Services");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
