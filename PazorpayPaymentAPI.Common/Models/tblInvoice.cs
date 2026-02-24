using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RazorpayPaymentAPI.Common.Models;

[Table("tblInvoice", Schema = "dbo")]
public partial class tblInvoice
{
    [Key]
    public Guid InvoiceId { get; set; }

    public Guid TransactionId { get; set; }

    public DateOnly InvoiceDate { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Amount { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? GSTAmount { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal NetAmount { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string InvoiceStatus { get; set; } = null!;

    public DateOnly? DueDate { get; set; }

    [ForeignKey("TransactionId")]
    [InverseProperty("tblInvoices")]
    public virtual tblTransaction Transaction { get; set; } = null!;

    [InverseProperty("Invoice")]
    public virtual ICollection<tblTaxDetail> tblTaxDetails { get; set; } = new List<tblTaxDetail>();
}
