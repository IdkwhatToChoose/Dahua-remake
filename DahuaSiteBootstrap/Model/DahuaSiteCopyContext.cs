using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DahuaSiteBootstrap.Model;

public partial class DahuaSiteCopyContext : DbContext
{
    public DahuaSiteCopyContext()
    {
    }

    public DahuaSiteCopyContext(DbContextOptions<DahuaSiteCopyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Dsfile> Dsfiles { get; set; }

    public virtual DbSet<Dstask> Dstasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOPCOMP;Initial Catalog=DahuaSiteCopy;Trusted_Connection=True;Encrypt=False;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Admin");

            entity.Property(e => e.AdminName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("admin_name");
            entity.Property(e => e.Password)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(e => e.RecieverAddress)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Reciever_Address");
            entity.Property(e => e.RecieverCity)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Reciever_City");
            entity.Property(e => e.RecieverEik)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("Reciever_EIK");
            entity.Property(e => e.RecieverName)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("Reciever_Name");
            entity.Property(e => e.RintroughDds)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("RINTrough_DDS");
        });

        modelBuilder.Entity<Dsfile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_File");

            entity.ToTable("DSFiles");

            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Content).HasMaxLength(50);
            entity.Property(e => e.DisplayName)
                .HasMaxLength(110)
                .HasColumnName("Display_name");
        });

        modelBuilder.Entity<Dstask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Tasks");

            entity.ToTable("DSTasks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdminId).HasColumnName("adminID");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Admin).WithMany(p => p.Dstasks)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK_DSTasks_Admin");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
