using Newtonsoft.Json;

namespace Orders.Domain.Entities
{
    public class Product : Entity
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("category")]
        public Category Category { get; set; }
        [JsonProperty("images")]
        public Images[] Images { get; set; }
        [JsonProperty("game")]
        public Game Game { get; set; }
        [JsonProperty("sale")]
        public bool Sale { get; set; }
        public decimal Discount { get; set; }
    }
}
