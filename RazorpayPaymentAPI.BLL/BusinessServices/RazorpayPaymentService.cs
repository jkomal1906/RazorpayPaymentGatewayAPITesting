using Newtonsoft.Json;
using Razorpay.Api;
using RazorpayPaymentAPI.BLL.Dependencies;
using RazorpayPaymentAPI.Common.DTOs;
using RazorpayPaymentAPI.DAL.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RazorpayPaymentAPI.BLL.BusinessServices
{
    public class RazorpayPaymentService : IRazorpayPaymentServices
    {
        private readonly string _key;
        private readonly string _secret;
        private readonly IPaymentTransactionService _service;
        private readonly string _webhookSecret;


        public RazorpayPaymentService(string key, string secret, string webhookSecret, IPaymentTransactionService service)
        {
            _key = key;
            _secret = secret;
            _service = service;
            _webhookSecret = webhookSecret;
        }


        // This method creates an order in Razorpay and saves the transaction details in the database.
        public async Task<string> CreateOrderAsync(CreateOrderRequestDto dto)
        {
            // GST Calculation
            dto.GstAmount = (dto.BaseAmount * dto.GstPercentage) / 100;
            dto.TotalAmount = dto.BaseAmount + dto.GstAmount - dto.DiscountAmount;
            dto.PaymentStatus = "Created";

            // Razorpay Integration
            var client = new RazorpayClient(_key, _secret);

            var options = new Dictionary<string, object>
            {
                { "amount", (int)(dto.TotalAmount * 100) }, // Razorpay works in paise
                { "currency", dto.Currency },
                { "payment_capture", 1 }
            };

            var razorpayOrder = client.Order.Create(options);

            string razorpayOrderId = razorpayOrder["id"].ToString();

            dto.RazorpayOrderId = $"RazorpayOrderId:{razorpayOrderId}";

            // Save in DB
            await _service.CreateOrder(dto);

            return razorpayOrderId;
        }

        public async Task<string> VerifyPaymentAsync(PaymentVerificationDto dto)
        {
            // Check Order Exists
            var order = await _service.GetOrderByRazorpayOrderIdAsync(dto.RazorpayOrderId);

            if (order == null)
                throw new Exception("Order not found.");

            if (order.PaymentStatus == "Paid")
                return "Already Paid";

            //  Verify Signature
            string generatedSignature = GenerateSignature(
                dto.RazorpayOrderId,
                dto.RazorpayPaymentId,
                _secret);

            if (!generatedSignature.Equals(dto.RazorpaySignature, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Invalid payment signature.");

            // Update Status → Authorized
            await _service.UpdatePaymentStatusAsync(
                dto.RazorpayOrderId,
                dto.RazorpayPaymentId,
                dto.RazorpaySignature,
                "Authorized",
                "Online");

            return "Authorized";
        }

        private string GenerateSignature(string orderId, string paymentId, string secret)
        {
            string payload = orderId + "|" + paymentId;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        //Webhook Verification
        public async Task ProcessWebhookAsync(string body, string signature)
        {
            if (!VerifyWebhookSignature(body, signature, _webhookSecret))
                throw new Exception("Invalid webhook signature.");

            var webhook = JsonConvert.DeserializeObject<RazorpayWebhookDto>(body);

            if (webhook?.Payload?.Payment?.Entity == null)
                return;

            var entity = webhook.Payload.Payment.Entity;

            string eventType = webhook.Event;
            string orderId = entity.OrderId;
            string paymentId = entity.Id;

            if (string.IsNullOrEmpty(orderId))
                return;

            switch (eventType)
            {
                case "payment.captured":
                    await HandlePaymentCapturedAsync(orderId, paymentId);
                    break;

                case "payment.failed":
                    await HandlePaymentFailedAsync(orderId);
                    break;
            }
        }

        private async Task HandlePaymentCapturedAsync(string orderId, string paymentId)
        {
            var order = await _service.GetOrderByRazorpayOrderIdAsync(orderId);

            if (order == null)
                return;

            if (order.PaymentStatus == "Paid")
                return; // prevents duplicate webhook processing

            await _service.UpdatePaymentStatusAsync(
                orderId,
                paymentId,
                null,
                "Paid",
                "Online");

            await _service.ActivateCourseEnrollmentAsync(orderId);
        }

        private async Task HandlePaymentFailedAsync(string orderId)
        {
            var order = await _service.GetOrderByRazorpayOrderIdAsync(orderId);

            if (order == null)
                return;

            if (order.PaymentStatus == "Paid")
                return;

            await _service.UpdatePaymentStatusAsync(
                orderId,
                null,
                null,
                "Failed",
                "Online");
        }

        private bool VerifyWebhookSignature(string body, string signature, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var bodyBytes = Encoding.UTF8.GetBytes(body);

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(bodyBytes);

            var generatedSignature = Convert.ToHexString(hash).ToLower();

            return generatedSignature.Equals(signature, StringComparison.OrdinalIgnoreCase);
        }

    }
}
   
   
 