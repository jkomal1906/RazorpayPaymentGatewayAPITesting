using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RazorpayPaymentAPI.Common.Models;

[Table("tblRecurrence", Schema = "dbo")]
public partial class tblRecurrence
{
    [Key]
    public Guid RecurrenceId { get; set; }

    public Guid TransactionId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Frequency { get; set; } = null!;

    public DateOnly? NextBillingDate { get; set; }

    public int? RecurrenceCount { get; set; }

    public int? RemainingRecurrences { get; set; }

    [ForeignKey("TransactionId")]
    [InverseProperty("tblRecurrences")]
    public virtual tblTransaction Transaction { get; set; } = null!;

    [InverseProperty("Recurrence")]
    public virtual ICollection<tblCourseEnrollmentPayment> tblCourseEnrollmentPayments { get; set; } = new List<tblCourseEnrollmentPayment>();
}
