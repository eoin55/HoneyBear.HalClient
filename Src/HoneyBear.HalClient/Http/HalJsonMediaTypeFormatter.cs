using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace HoneyBear.HalClient.Http
{
    internal class HalJsonMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public HalJsonMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+json"));
            SerializerSettings.DateParseHandling = DateParseHandling.None;
        }
    }
}