using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorpayPaymentAPI.Common.Models
{
    public class OrderModel
    {
        public string RazorpayOrderId { get; set; }
        public string PaymentStatus { get; set; }
    }
}
