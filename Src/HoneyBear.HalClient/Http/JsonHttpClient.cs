using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HoneyBear.HalClient.Http
{
    internal class JsonHttpClient : IJsonHttpClient
    {
        public JsonHttpClient(HttpClient client)
        {
            HttpClient = client;
            AcceptJson();
        }

        public HttpClient HttpClient { get; }

        public Task<HttpResponseMessage> GetAsync(string uri)
            => HttpClient.GetAsync(uri);

        public Task<HttpResponseMessage> PostAsync<T>(string uri, T value)
            => HttpClient.PostAsJsonAsync(uri, value);

        public Task<HttpResponseMessage> PutAsync<T>(string uri, T value)
            => HttpClient.PutAsJsonAsync(uri, value);

        public Task<HttpResponseMessage> DeleteAsync(string uri)
            => HttpClient.DeleteAsync(uri);

        private void AcceptJson()
        {
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}