using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HoneyBear.HalClient.Models;

namespace HoneyBear.HalClient
{
    /// <summary>
    /// Provides asynchronous extension methods for <see cref="IHalClient"/>.
    /// </summary>
    public static class HalClientAsyncExtensions
    {
        /// <summary>
        /// Returns the most recently navigated resource of the specified type. 
        /// </summary>
        /// <typeparam name="T">The type of the resource to return.</typeparam>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <returns>The most recent navigated resource of the specified type.</returns>
        /// <exception cref="NoActiveResource" />
        public static IResource<T> Item<T>(this Task<IHalClient> hal) where T : class, new() =>
            hal.Result.Item<T>();

        /// <summary>
        /// Returns the list of embedded resources in the most recently navigated resource.
        /// </summary>
        /// <typeparam name="T">The type of the resource to return.</typeparam>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <returns>The list of embedded resources in the most recently navigated resource.</returns>
        /// <exception cref="NoActiveResource" />
        public static IEnumerable<IResource<T>> Items<T>(this Task<IHalClient> hal) where T : class, new() =>
            hal.Result.Items<T>();

        /// <summary>
        /// Makes a HTTP GET request to the default URI and stores the returned resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        public static Task<IHalClient> RootAsync(this IHalClient hal) =>
            hal.RootAsync(string.Empty);

        /// <summary>
        /// Makes a HTTP GET request to the given URL and stores the returned resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="href">The URI to request.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        public static Task<IHalClient> RootAsync(this IHalClient hal, string href)
        {
            var client = (HalClient) hal;
            return client.ExecuteRootAsync(href);
        }

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> hal, string rel) =>
            hal.GetAsync(rel, null, null);

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> hal, string rel, string curie) =>
            hal.GetAsync(rel, null, curie);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> hal, string rel, object parameters) =>
            hal.GetAsync(rel, parameters, null);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static async Task<IHalClient> GetAsync(this Task<IHalClient> hal, string rel, object parameters, string curie)
        {
            var client = (HalClient) await hal;
            return await client.ExecuteGetAsync(rel, parameters, curie);
        }


        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> hal, IResource resource, string rel) =>
            hal.GetAsync(resource, rel, null, null);

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> hal, IResource resource, string rel, string curie) =>
            hal.GetAsync(resource, rel, null, curie);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> GetAsync(this Task<IHalClient> hal, IResource resource, string rel, object parameters) =>
            hal.GetAsync(resource, rel, parameters, null);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static async Task<IHalClient> GetAsync(this Task<IHalClient> hal, IResource resource, string rel, object parameters, string curie)
        {
            var client = (HalClient) await hal;
            return await client.ExecuteGetAsync(resource, rel, parameters, curie);
        }

        /// <summary>
        /// Makes a HTTP POST request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> PostAsync(this Task<IHalClient> hal, string rel, object value) =>
            hal.PostAsync(rel, value, null, null);

        /// <summary>
        /// Makes a HTTP POST request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> PostAsync(this Task<IHalClient> hal, string rel, object value, string curie) =>
            hal.PostAsync(rel, value, null, curie);

        /// <summary>
        /// Makes a HTTP POST request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> PostAsync(this Task<IHalClient> hal, string rel, object value, object parameters) =>
            hal.PostAsync(rel, value, parameters, null);

        /// <summary>
        /// Makes a HTTP POST request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static async Task<IHalClient> PostAsync(this Task<IHalClient> hal, string rel, object value, object parameters, string curie)
        {
            var client = (HalClient) await hal;
            return await client.ExecutePostAsync(rel, value, parameters, curie);
        }

        /// <summary>
        /// Makes a HTTP PUT request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> PutAsync(this Task<IHalClient> hal, string rel, object value) =>
            hal.PutAsync(rel, value, null, null);

        /// <summary>
        /// Makes a HTTP PUT request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> PutAsync(this Task<IHalClient> hal, string rel, object value, string curie) =>
            hal.PutAsync(rel, value, null, curie);

        /// <summary>
        /// Makes a HTTP PUT request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> PutAsync(this Task<IHalClient> hal, string rel, object value, object parameters) =>
            hal.PutAsync(rel, value, parameters, null);

        /// <summary>
        /// Makes a HTTP PUT request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static async Task<IHalClient> PutAsync(this Task<IHalClient> hal, string rel, object value, object parameters, string curie)
        {
            var client = (HalClient) await hal;
            return await client.ExecutePutAsync(rel, value, parameters, curie);
        }

        /// <summary>
        /// Makes a HTTP PATCH request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> PatchAsync(this Task<IHalClient> hal, string rel, object value) =>
            hal.PatchAsync(rel, value, null, null);

        /// <summary>
        /// Makes a HTTP PATCH request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> PatchAsync(this Task<IHalClient> hal, string rel, object value, string curie) =>
            hal.PatchAsync(rel, value, null, curie);

        /// <summary>
        /// Makes a HTTP PATCH request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> PatchAsync(this Task<IHalClient> hal, string rel, object value, object parameters) =>
            hal.PatchAsync(rel, value, parameters, null);

        /// <summary>
        /// Makes a HTTP PATCH request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static async Task<IHalClient> PatchAsync(this Task<IHalClient> hal, string rel, object value, object parameters, string curie)
        {
            var client = (HalClient) await hal;
            return await client.ExecutePatchAsync(rel, value, parameters, curie);
        }

        /// <summary>
        /// Makes a HTTP DELETE request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> DeleteAsync(this Task<IHalClient> hal, string rel) =>
            hal.DeleteAsync(rel, null, null);

        /// <summary>
        /// Makes a HTTP DELETE request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> DeleteAsync(this Task<IHalClient> hal, string rel, string curie) =>
            hal.DeleteAsync(rel, null, curie);

        /// <summary>
        /// Makes a HTTP DELETE request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static Task<IHalClient> DeleteAsync(this Task<IHalClient> hal, string rel, object parameters) =>
            hal.DeleteAsync(rel, parameters, null);

        /// <summary>
        /// Makes a HTTP DELETE request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="hal">The instance of <see cref="IHalClient"/> to extend.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="AggregateException" />
        public static async Task<IHalClient> DeleteAsync(this Task<IHalClient> hal, string rel, object parameters, string curie)
        {
            var client = (HalClient) await hal;
            return await client.ExecuteDeleteAsync(rel, parameters, curie);
        }
    }
}