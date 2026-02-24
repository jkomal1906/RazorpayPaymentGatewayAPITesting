using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RazorpayPaymentAPI.Common.Models;

[Table("tblUserTransactionDiscount", Schema = "dbo")]
public partial class tblUserTransactionDiscount
{
    [Key]
    public Guid UserTransactionDiscountId { get; set; }

    public Guid TransactionId { get; set; }

    public Guid DiscountId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string DiscountType { get; set; } = null!;

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? DiscountAmount { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? DiscountPercentage { get; set; }

    [ForeignKey("TransactionId")]
    [InverseProperty("tblUserTransactionDiscounts")]
    public virtual tblTransaction Transaction { get; set; } = null!;
}
