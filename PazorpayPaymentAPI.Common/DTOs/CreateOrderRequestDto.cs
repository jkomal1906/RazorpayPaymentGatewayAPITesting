using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RazorpayPaymentAPI.Common.DTOs
{
    public class CreateOrderRequestDto
    {
        [JsonIgnore]
        public Guid _transactionId;

        public Guid TransactionId
        {
            get
            {
                if (_transactionId == Guid.Empty)
                    _transactionId = Guid.NewGuid();
                return _transactionId;
            }
            set { _transactionId = value; }
        }

        [Required]
        public Guid UserId { get; set; }

        [JsonIgnore]
        public Guid _referenceId;

        public Guid ReferenceId
        {
            get
            {
                if (_referenceId == Guid.Empty)
                    _referenceId = Guid.NewGuid();
                return _referenceId;
            }
            set { _referenceId = value; }
        }

        [Required]
        public int PaymentTypeId { get; set; }

        [Required]
        public decimal BaseAmount { get; set; }

        public decimal GstPercentage { get; set; }

        public decimal GstAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }
        public string PaymentMode { get; set; } = "Online";

        public string Currency { get; set; } = "INR";

        public string? Email { get; set; }
        public string? PaymentStatus { get; set; }

        public string? RazorpayOrderId { get; set; }

        // Internal OrderId (GUID)

        [JsonIgnore]
        public Guid _orderId;

        public Guid OrderId
        {
            get
            {
                if (_orderId == Guid.Empty)
                    _orderId = Guid.NewGuid();
                return _orderId;
            }
            set { _orderId = value; }
        }
    }
}
