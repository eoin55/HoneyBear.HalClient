using Newtonsoft.Json;

namespace HoneyBear.HalClient.Models
{
    public interface ILink : INode
    {
        [JsonProperty("templated", DefaultValueHandling = DefaultValueHandling.Ignore)]
        bool Templated { get; set; }
    }
}