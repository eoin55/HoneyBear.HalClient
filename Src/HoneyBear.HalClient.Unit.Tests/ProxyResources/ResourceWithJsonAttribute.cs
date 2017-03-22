using Newtonsoft.Json;
using Version = HoneyBear.HalClient.Unit.Tests.ProxyResources.Version;

namespace HoneyBear.HalClient.Unit.Tests.ProxyResources
{
    internal class ResourceWithJsonAttribute
    {
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string PropertyWithJsonAttribute { get; set; }
    }
}
