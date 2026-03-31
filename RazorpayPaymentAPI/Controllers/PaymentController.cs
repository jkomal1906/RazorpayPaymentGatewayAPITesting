using Microsoft.AspNetCore.Mvc;
using RazorpayPaymentAPI.BLL.BusinessServices;
using RazorpayPaymentAPI.BLL.Dependencies;
using RazorpayPaymentAPI.Common.DTOs;
using System.Text;
namespace RazorpayPaymentAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
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
            try
            {
                Request.EnableBuffering();
                Request.ReadFromJsonAsync<object>(); // Read the body to enable buffering for subsequent reads
                string body = await new StreamReader(Request.Body).ReadToEndAsync();

                Console.WriteLine("===== WEBHOOK RECEIVED =====");
                Console.WriteLine(body);

                var signature = Request.Headers["X-Razorpay-Signature"].FirstOrDefault();

                Console.WriteLine("Signature: " + signature);

                if (string.IsNullOrEmpty(body))
                {
                    Console.WriteLine("Body is empty");
                    return Ok();
                }

                await _paymentService.ProcessWebhookAsync(body, signature);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ok();
            }
        }
    }
}
