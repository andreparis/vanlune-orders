using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Domain.Entities
{
    public class Images : Entity
    {
        [JsonProperty("image_id")]
        public int ImageId { get; set; }
        [JsonProperty("alt")]
        public string Alt { get; set; }
        [JsonProperty("src")]
        public string Src { get; set; }
        [JsonProperty("variant_id")]
        public int[] VariantId { get; set; }
    }
}
