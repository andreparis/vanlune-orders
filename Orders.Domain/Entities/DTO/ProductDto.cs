using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Domain.Entities.DTO
{
    public class ProductDto : Product
    {
        [JsonProperty("variants")]
        public Variants[] Variants { get; set; }
        [JsonProperty("images")]
        public Images[] Images { get; set; }
        [JsonProperty("tags")]
        public string[] Tags { get; set; }        
    }
}
