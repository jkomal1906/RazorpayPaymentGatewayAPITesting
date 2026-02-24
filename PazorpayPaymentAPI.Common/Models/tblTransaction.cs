using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RazorpayPaymentAPI.Common.Models;

[Table("tblTransaction", Schema = "dbo")]
public partial class tblTransaction
{
    [Key]
    public Guid TransactionId { get; set; }

    public Guid UserId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string TransactionType { get; set; } = null!;

    public Guid ReferenceId { get; set; }

    public int PaymentTypeId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string PaymentStatus { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreatedOn { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string PaymentMode { get; set; } = null!;

    public bool? DiscountApplied { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? DiscountAmount { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? PaymentId { get; set; }

    [StringLength(10)]
    public string? Currency { get; set; }

    [StringLength(200)]
    public string? BankName { get; set; }

    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? ContactNumber { get; set; }

    public string? Signature { get; set; }

    public string? RazorpayOrderId { get; set; }

    [InverseProperty("Transaction")]
    public virtual ICollection<tblCourseEnrollmentPayment> tblCourseEnrollmentPayments { get; set; } = new List<tblCourseEnrollmentPayment>();

    [InverseProperty("Transaction")]
    public virtual ICollection<tblInvoice> tblInvoices { get; set; } = new List<tblInvoice>();

    [InverseProperty("Transaction")]
    public virtual ICollection<tblRecurrence> tblRecurrences { get; set; } = new List<tblRecurrence>();

    [InverseProperty("Transaction")]
    public virtual ICollection<tblTeacherSubscriptionPayment> tblTeacherSubscriptionPayments { get; set; } = new List<tblTeacherSubscriptionPayment>();

    [InverseProperty("Transaction")]
    public virtual ICollection<tblUserTransactionDiscount> tblUserTransactionDiscounts { get; set; } = new List<tblUserTransactionDiscount>();
}
