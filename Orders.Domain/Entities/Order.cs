using Newtonsoft.Json;
using Orders.Domain.Entities.Client;
using Orders.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Orders.Domain.Entities
{
    public class Order : Entity
    {
        [JsonProperty("product")]
        public Product Product { get; set; }
        [JsonProperty("status")]
        public Status? Status { get; set; }
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("payment")]
        public Payment? Payment { get; set; }
        [JsonProperty("createdAt")]
        public DateTime? CreatedAt { get; set; }
        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
        [JsonProperty("variant")]
        public Variants Variant { get; set; }
        [JsonProperty("attendent")]
        public User Attendent { get; set; }
        [JsonProperty("discount")]
        public decimal Discount { get; set; }
        [JsonProperty("customizes")]
        public IEnumerable<Entities.ExProduct.Customize> Customizes { get; set; }
        [JsonProperty("paymentStatus")]
        public string PaymentStatus { get; set; }
        [JsonProperty("externalId")]
        public string ExternalId { get; set; }

        public bool IsValid()
        {
            return User != null &&
                User.Id > 0 &&
                Product != null &&
                Product.Id > 0;
        }
    }
}
