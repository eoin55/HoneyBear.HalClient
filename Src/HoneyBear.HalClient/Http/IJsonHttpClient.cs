using System.Net.Http;
using System.Threading.Tasks;

namespace HoneyBear.HalClient.Http
{
    internal interface IJsonHttpClient
    {
        HttpClient HttpClient { get; }
        Task<HttpResponseMessage> GetAsync(string uri);
        Task<HttpResponseMessage> PostAsync<T>(string uri, T value);
        Task<HttpResponseMessage> PutAsync<T>(string uri, T value);
        Task<HttpResponseMessage> DeleteAsync(string uri);
    }
}