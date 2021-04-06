using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orders.Domain.Entities.ExProduct
{
    public class Customize : Entity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public IList<CustomizeValue> Value { get; set; }
        [JsonProperty("game")]
        public Game Game { get; set; }
    }

    public class CustomizeValue
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("factor")]
        public decimal Factor { get; set; }
    }
}
