using System;
using System.Net;

namespace HoneyBear.HalClient.Models
{
    /// <summary>
    /// Indicates that a HTTP request returned a non-successful response.
    /// </summary>
    public sealed class HttpRequestFailed : Exception
    {
        /// <summary>
        /// Creates an instance of the <see cref="HttpRequestFailed"/> exception.
        /// </summary>
        /// <param name="statusCode">The HTTP status code returned in the HTTP response.</param>
        public HttpRequestFailed(HttpStatusCode statusCode)
            : base($"HTTP request returned non-successful HTTP status code:{statusCode}")
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// The HTTP status code returned in the HTTP response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }
    }
}