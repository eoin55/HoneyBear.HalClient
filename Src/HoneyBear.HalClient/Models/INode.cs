using Newtonsoft.Json;

namespace HoneyBear.HalClient.Models
{
    public interface INode
    {
        [JsonIgnore]
        string Rel { get; set; }

        [JsonProperty("href")]
        string Href { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        string Name { get; set; }
    }
}