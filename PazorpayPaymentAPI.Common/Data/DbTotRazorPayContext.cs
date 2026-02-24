using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RazorpayPaymentAPI.Common.Models;

namespace RazorpayPaymentAPI.Common.Data;

public partial class DbTotRazorPayContext : DbContext
{
    public DbTotRazorPayContext()
    {
    }

    public DbTotRazorPayContext(DbContextOptions<DbTotRazorPayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<tblCourseEnrollmentPayment> tblCourseEnrollmentPayments { get; set; }

    public virtual DbSet<tblInvoice> tblInvoices { get; set; }

    public virtual DbSet<tblRecurrence> tblRecurrences { get; set; }

    public virtual DbSet<tblTaxDetail> tblTaxDetails { get; set; }

    public virtual DbSet<tblTeacherSubscriptionPayment> tblTeacherSubscriptionPayments { get; set; }

    public virtual DbSet<tblTransaction> tblTransactions { get; set; }

    public virtual DbSet<tblUserTransactionDiscount> tblUserTransactionDiscounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=115.124.108.190;Database=db_totFPS;User Id=totuserservice;Password=Eiyn9634*;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("totuserservice");

        modelBuilder.Entity<tblCourseEnrollmentPayment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentPaymentId).HasName("PK__tblCours__2E50AD2702B8FB05");

            entity.Property(e => e.EnrollmentPaymentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsRecurring).HasDefaultValue(false);

            entity.HasOne(d => d.Recurrence).WithMany(p => p.tblCourseEnrollmentPayments).HasConstraintName("FK_tblCourseEnrollmentPayment_tblRecurrence");

            entity.HasOne(d => d.Transaction).WithMany(p => p.tblCourseEnrollmentPayments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCourseEnrollmentPayment_tblTransaction");
        });

        modelBuilder.Entity<tblInvoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__tblInvoi__D796AAB5974B06DB");

            entity.Property(e => e.InvoiceId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Transaction).WithMany(p => p.tblInvoices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblInvoice_tblTransaction");
        });

        modelBuilder.Entity<tblRecurrence>(entity =>
        {
            entity.HasKey(e => e.RecurrenceId).HasName("PK__tblRecur__9D537B554751211D");

            entity.Property(e => e.RecurrenceId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Transaction).WithMany(p => p.tblRecurrences)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblRecurrence_tblTransaction");
        });

        modelBuilder.Entity<tblTaxDetail>(entity =>
        {
            entity.HasKey(e => e.TaxId).HasName("PK__tblTaxDe__711BE0AC2DABFB43");

            entity.Property(e => e.TaxId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Invoice).WithMany(p => p.tblTaxDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTaxDetails_tblInvoice");
        });

        modelBuilder.Entity<tblTeacherSubscriptionPayment>(entity =>
        {
            entity.HasKey(e => e.SubscriptionPaymentId).HasName("PK__tblTeach__8BE6C43B3A611F19");

            entity.Property(e => e.SubscriptionPaymentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsRecurring).HasDefaultValue(true);

            entity.HasOne(d => d.Transaction).WithMany(p => p.tblTeacherSubscriptionPayments).HasConstraintName("FK_tblTeacherSubscriptionPayment_tblTransaction");
        });

        modelBuilder.Entity<tblTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__tblTrans__55433A6B40E90545");

            entity.Property(e => e.TransactionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.DiscountApplied).HasDefaultValue(false);
        });

        modelBuilder.Entity<tblUserTransactionDiscount>(entity =>
        {
            entity.HasKey(e => e.UserTransactionDiscountId).HasName("PK__tblUserT__8DFCFDE1BAB56FA9");

            entity.Property(e => e.UserTransactionDiscountId).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Transaction).WithMany(p => p.tblUserTransactionDiscounts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblUserTransactionDiscount_tblTransaction");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
