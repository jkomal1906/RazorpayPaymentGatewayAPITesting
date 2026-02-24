using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RazorpayPaymentAPI.Common.Models;

[Table("tblCourseEnrollmentPayment", Schema = "dbo")]
public partial class tblCourseEnrollmentPayment
{
    [Key]
    public Guid EnrollmentPaymentId { get; set; }

    public Guid EnrollmentId { get; set; }

    public Guid TransactionId { get; set; }

    public bool? IsRecurring { get; set; }

    public Guid? RecurrenceId { get; set; }

    [ForeignKey("RecurrenceId")]
    [InverseProperty("tblCourseEnrollmentPayments")]
    public virtual tblRecurrence? Recurrence { get; set; }

    [ForeignKey("TransactionId")]
    [InverseProperty("tblCourseEnrollmentPayments")]
    public virtual tblTransaction Transaction { get; set; } = null!;
}
