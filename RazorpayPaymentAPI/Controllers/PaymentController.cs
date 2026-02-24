using Microsoft.AspNetCore.Mvc;
using RazorpayPaymentAPI.BLL.BusinessServices;
using RazorpayPaymentAPI.BLL.Dependencies;
using RazorpayPaymentAPI.Common.DTOs;
using System.Text;
namespace RazorpayPaymentAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IRazorpayPaymentServices _paymentService;
        public PaymentController(IRazorpayPaymentServices paymentService)
        {
            _paymentService = paymentService;
        }

        //Create Razorpay Order
        [HttpPost("r/create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDto orderDto)
        {
            if (orderDto == null)
                return BadRequest("The orderDto field is required.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var razorpayOrderId = await _paymentService.CreateOrderAsync(orderDto);

                return Ok(new
                {
                    message = "Order created successfully.",
                    razorpayOrderId,
                    internalOrderId = orderDto.OrderId,
                    amount = orderDto.TotalAmount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request.");
            }
        }

        //Payment Verification
        [HttpPost("r/verify-payment")]
        public async Task<IActionResult> VerifyPayment([FromBody] PaymentVerificationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _paymentService.VerifyPaymentAsync(dto);

                return Ok(new
                {
                    message = "Payment authorized successfully.",
                    status = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        //Webhook Endpoint
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            // Enable reading body multiple times (important)
            Request.EnableBuffering();

            string body;
            using (var reader = new StreamReader(
                Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true))
            {
                body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
            }

            var signature = Request.Headers["X-Razorpay-Signature"].ToString();

            if (string.IsNullOrEmpty(signature))
                return BadRequest("Missing webhook signature.");

            await _paymentService.ProcessWebhookAsync(body, signature);

            return Ok();
        }

    }
}
