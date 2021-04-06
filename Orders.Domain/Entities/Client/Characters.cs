using Newtonsoft.Json;

namespace Orders.Domain.Entities.Client
{
    public class Characters
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("game")]
        public string Game { get; set; }
        [JsonProperty("server")]
        public string Server { get; set; }
    }
}
