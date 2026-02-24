using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorpayPaymentAPI.Common.DTOs
{
    public class RazorpayWebhookDto
    {
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("payload")]
        public RazorpayWebhookPayload Payload { get; set; }
    }

    public class RazorpayWebhookPayload
    {
        [JsonProperty("payment")]
        public RazorpayPaymentContainer Payment { get; set; }
    }

    public class RazorpayPaymentContainer
    {
        [JsonProperty("entity")]
        public RazorpayPaymentEntity Entity { get; set; }
    }

    public class RazorpayPaymentEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

}
