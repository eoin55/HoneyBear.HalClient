using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace HoneyBear.HalClient.Http
{
    internal static class HttpClientExtensions
    {
        public const string MimeJson = "application/json";

        public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(client.BaseAddress, requestUri),
                Content = content
            };

            return client.SendAsync(request);
        }
        
        public static Task<HttpResponseMessage> PatchJsonAsync(this HttpClient client, string requestUri, Type type, object value) =>
            client.PatchAsync(requestUri, new ObjectContent(type, value, new JsonMediaTypeFormatter(), MimeJson));
    }
}
