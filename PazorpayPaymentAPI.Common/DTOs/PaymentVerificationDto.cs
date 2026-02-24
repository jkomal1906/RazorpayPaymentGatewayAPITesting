using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorpayPaymentAPI.Common.DTOs
{
    public class PaymentVerificationDto
    {
        [Required]
        public string RazorpayOrderId { get; set; }

        [Required]
        public string RazorpayPaymentId { get; set; }

        [Required]
        public string RazorpaySignature { get; set; }
    }
}
