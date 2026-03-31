using Microsoft.EntityFrameworkCore;
using RazorpayPaymentAPI.BLL.BusinessServices;
using RazorpayPaymentAPI.BLL.Dependencies;
using RazorpayPaymentAPI.Common.Data;
using RazorpayPaymentAPI.DAL.IServices;
using RazorpayPaymentAPI.DAL.Services;

namespace RazorpayPaymentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DB Context
            builder.Services.AddDbContext<DbTotRazorPayContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // DAL
            builder.Services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();

            // Razorpay Service
            builder.Services.AddScoped<IRazorpayPaymentServices>(provider =>
            {
                var paymentTransactionService = provider.GetRequiredService<IPaymentTransactionService>();
                var key = builder.Configuration["Razorpay:Key"];
                var secret = builder.Configuration["Razorpay:Secret"];
                var webhookSecret = builder.Configuration["Razorpay:WebhookSecret"];

                return new RazorpayPaymentService(key, secret, webhookSecret, paymentTransactionService);
            });

            // Kestrel limits
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = null;
                options.Limits.MinRequestBodyDataRate = null;
                options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
            });


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Enable request buffering globally
            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });

            //DO NOT use HTTPS redirect with ngrok
            // app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}