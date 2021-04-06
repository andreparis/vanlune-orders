using Newtonsoft.Json;

namespace Orders.Domain.Entities
{
    public class Category : Entity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
