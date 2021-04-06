using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orders.Domain.Entities
{
    public class Variants : Entity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("factor")]
        public decimal Factor { get; set; }
    }
}
