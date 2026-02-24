using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RazorpayPaymentAPI.Common.Models;

[Table("tblTeacherSubscriptionPayment", Schema = "dbo")]
public partial class tblTeacherSubscriptionPayment
{
    [Key]
    public Guid SubscriptionPaymentId { get; set; }

    public Guid TeacherId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? SubscriptionType { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Status { get; set; }

    public Guid? TransactionId { get; set; }

    public bool? IsRecurring { get; set; }

    [ForeignKey("TransactionId")]
    [InverseProperty("tblTeacherSubscriptionPayments")]
    public virtual tblTransaction? Transaction { get; set; }
}
