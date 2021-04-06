using Newtonsoft.Json;
using System.Collections.Generic;

namespace Orders.Domain.Entities.Client
{
    public class User : Entity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("characters")]
        public IEnumerable<Characters> Characters { get; set; }
    }
}
