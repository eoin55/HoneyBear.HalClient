using System.Net.Http;
using System.Threading.Tasks;

namespace HoneyBear.HalClient.Http
{
    /// <summary>
    /// Provides a wrapper for <see cref="System.Net.Http.HttpClient"/> that processes JSON HTTP requests and responses.
    /// </summary>
    public interface IJsonHttpClient
    {
        /// <summary>
        /// A getter for the wrapped instance of <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.</returns>
        Task<HttpResponseMessage> GetAsync(string uri);

        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"/>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <param name="value">The HTTP request content sent to the server.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.</returns>
        Task<HttpResponseMessage> PostAsync<T>(string uri, T value);

        /// <summary>
        /// Send a PUT request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <typeparam name="T"/>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <param name="value">The HTTP request content sent to the server.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.</returns>
        Task<HttpResponseMessage> PutAsync<T>(string uri, T value);

        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="uri">The Uri the request is sent to.</param>
        /// <returns>Returns <see cref="T:System.Threading.Tasks.Task`1"/>.The task object representing the asynchronous operation.</returns>
        Task<HttpResponseMessage> DeleteAsync(string uri);
    }
}
