using RazorpayPaymentAPI.Common.DTOs;
using RazorpayPaymentAPI.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorpayPaymentAPI.DAL.IServices
{
    public interface IPaymentTransactionService
    {
        Task CreateOrder(CreateOrderRequestDto dto);
        Task<OrderModel> GetOrderByRazorpayOrderIdAsync(string razorpayOrderId);
        Task UpdatePaymentStatusAsync(string razorpayOrderId, string razorpayPaymentId, string signature, string paymentStatus, string paymentMode);
        Task ActivateCourseEnrollmentAsync(string razorpayOrderId);
    }
}
