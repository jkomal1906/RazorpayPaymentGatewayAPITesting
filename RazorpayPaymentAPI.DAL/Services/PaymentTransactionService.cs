using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RazorpayPaymentAPI.Common.Data;
using RazorpayPaymentAPI.Common.DTOs;
using RazorpayPaymentAPI.Common.Models;
using RazorpayPaymentAPI.DAL.IServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorpayPaymentAPI.DAL.Services
{
    public class PaymentTransactionService : IPaymentTransactionService
    {
        private readonly DbTotRazorPayContext _context;
        public readonly ILogger<PaymentTransactionService> _logger;

        public PaymentTransactionService(DbTotRazorPayContext context, ILogger<PaymentTransactionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task CreateOrder(CreateOrderRequestDto dto)
        {
            using var connection = _context.Database.GetDbConnection() as SqlConnection;

            if (connection == null)
            {
                _logger.LogError("Invalid SQL connection.");
                throw new InvalidOperationException("Invalid SQL connection.");
            }

            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_InsertOrder", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@TransactionId", dto.TransactionId);
            cmd.Parameters.AddWithValue("@UserId", dto.UserId);
            cmd.Parameters.AddWithValue("@TransactionType", "OnlinePayment");
            cmd.Parameters.AddWithValue("@ReferenceId", dto.ReferenceId);
            cmd.Parameters.AddWithValue("@PaymentTypeId", dto.PaymentTypeId);
            cmd.Parameters.AddWithValue("@Amount", dto.TotalAmount);
            cmd.Parameters.AddWithValue("@PaymentStatus", dto.PaymentStatus);
            cmd.Parameters.AddWithValue("@PaymentMode", dto.PaymentMode ?? "Online");
            cmd.Parameters.AddWithValue("@DiscountApplied", dto.DiscountAmount > 0);
            cmd.Parameters.AddWithValue("@DiscountAmount", dto.DiscountAmount);
            cmd.Parameters.AddWithValue("@OrderId", dto.OrderId);
            cmd.Parameters.AddWithValue("@Currency", dto.Currency);
            cmd.Parameters.AddWithValue("@Email", (object?)dto.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RazorpayOrderId", (object?)dto.RazorpayOrderId ?? DBNull.Value);

            await cmd.ExecuteNonQueryAsync();
        }

        // This method updates the payment status in the database after verifying the payment.
        public async Task<OrderModel> GetOrderByRazorpayOrderIdAsync(string razorpayOrderId)
        {
            using var connection = _context.Database.GetDbConnection() as SqlConnection;

            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_GetOrderByRazorpayOrderId", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RazorpayOrderId", razorpayOrderId);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new OrderModel
                {
                    RazorpayOrderId = reader["RazorpayOrderId"].ToString(),
                    PaymentStatus = reader["PaymentStatus"].ToString()
                };
            }

            return null;
        }

        public async Task UpdatePaymentStatusAsync(string razorpayOrderId, string razorpayPaymentId, string signature, string paymentStatus, string paymentMode)
        {
            using var connection = _context.Database.GetDbConnection() as SqlConnection;

            await connection.OpenAsync();

            using var cmd = new SqlCommand("SP_UpdatePaymentStatus", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@RazorpayOrderId", razorpayOrderId);
            cmd.Parameters.AddWithValue("@RazorpayPaymentId", razorpayPaymentId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Signature", signature ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
            cmd.Parameters.AddWithValue("@PaymentMode", paymentMode);

            await cmd.ExecuteNonQueryAsync();
        }

        //Webhook handler to update payment status in case of delayed webhook calls or failed client-side updates.
        public async Task ActivateCourseEnrollmentAsync(string razorpayOrderId)
        {
            var connection = (SqlConnection)_context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var command = new SqlCommand("SP_ActivateCourseEnrollment", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@RazorpayOrderId", SqlDbType.NVarChar, 100)
                   .Value = razorpayOrderId;

            command.CommandTimeout = 60;

            await command.ExecuteNonQueryAsync();
        }
    }
}
