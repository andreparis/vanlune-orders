using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Domain.Entities
{
    public class Game : Entity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
