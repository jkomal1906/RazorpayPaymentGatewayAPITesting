using RazorpayPaymentAPI.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorpayPaymentAPI.BLL.Dependencies
{
    public interface IRazorpayPaymentServices
    {
        Task<string> CreateOrderAsync(CreateOrderRequestDto dto);
        Task<string> VerifyPaymentAsync(PaymentVerificationDto dto);
        Task ProcessWebhookAsync(string body, string signature);
    }
}
