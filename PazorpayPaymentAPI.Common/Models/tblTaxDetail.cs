using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RazorpayPaymentAPI.Common.Models;

[Table("tblTaxDetails", Schema = "dbo")]
public partial class tblTaxDetail
{
    [Key]
    public Guid TaxId { get; set; }

    public Guid InvoiceId { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? CGST { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? SGST { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? IGST { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? UGST { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? TotalTax { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("tblTaxDetails")]
    public virtual tblInvoice Invoice { get; set; } = null!;
}
