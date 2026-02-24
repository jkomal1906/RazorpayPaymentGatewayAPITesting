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

            // Register DbContext
            builder.Services.AddDbContext<DbTotRazorPayContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register DAL service
            builder.Services.AddScoped<IPaymentTransactionService, PaymentTransactionService>();

            // Register BLL Razorpay service with factory
            builder.Services.AddScoped<IRazorpayPaymentServices>(provider =>
            {
                var paymentTransactionService = provider.GetRequiredService<IPaymentTransactionService>();
                var key = builder.Configuration["Razorpay:Key"];
                var secret = builder.Configuration["Razorpay:Secret"];
                var webhookSecret = builder.Configuration["Razorpay:WebhookSecret"];
                return new RazorpayPaymentService(key, secret,webhookSecret ,paymentTransactionService);
            });

            // Add controllers & swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
