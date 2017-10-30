using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using HoneyBear.HalClient.Http;
using HoneyBear.HalClient.Models;
using Tavis.UriTemplates;

namespace HoneyBear.HalClient
{
    /// <summary>
    /// A lightweight fluent .NET client for navigating and consuming HAL APIs.
    /// </summary>
    public class HalClient : IHalClient
    {
        private readonly IJsonHttpClient _client;
        private readonly IEnumerable<MediaTypeFormatter> _formatters;
        private readonly IEnumerable<IResource> _current = Enumerable.Empty<IResource>();

        private static readonly ICollection<MediaTypeFormatter> _defaultFormatters =
            new[] {new HalJsonMediaTypeFormatter()};

        /// <summary>
        /// Creates an instance of the <see cref="HalClient"/> class.
        /// </summary>
        /// <param name="client">The <see cref="System.Net.Http.HttpClient"/> to use.</param>
        /// <param name="formatters">
        /// Specifies the list of <see cref="MediaTypeFormatter"/>s to use.
        /// Default is <see cref="HalJsonMediaTypeFormatter"/>.
        /// </param>
        public HalClient(
            HttpClient client,
            ICollection<MediaTypeFormatter> formatters)
        {
            _client = new JsonHttpClient(client);
            _formatters = formatters == null || !formatters.Any() ? _defaultFormatters : formatters;
        }

        /// <summary>
        /// Creates an instance of the <see cref="HalClient"/> class.
        /// </summary>
        /// <param name="client">The <see cref="System.Net.Http.HttpClient"/> to use.</param>
        public HalClient(
            HttpClient client)
            : this(client, _defaultFormatters)
        {
            
        }

        /// <summary>
        /// Creates an instance of the <see cref="HalClient"/> class.
        /// Uses a default instance of <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        public HalClient()
            : this(new HttpClient())
        {
            
        }

        /// <summary>
        /// Creates an instance of the <see cref="HalClient"/> class.
        /// </summary>
        /// <param name="client">The implementation of <see cref="IJsonHttpClient"/> to use.</param>
        /// <param name="formatters">
        /// Specifies the list of <see cref="MediaTypeFormatter"/>s to use.
        /// Default is <see cref="HalJsonMediaTypeFormatter"/>.
        /// </param>
        public HalClient(
            IJsonHttpClient client,
            ICollection<MediaTypeFormatter> formatters)
        {
            _client = client;
            _formatters = formatters == null || !formatters.Any() ? _defaultFormatters : formatters;
        }

        /// <summary>
        /// Creates an instance of the <see cref="HalClient"/> class.
        /// </summary>
        /// <param name="client">The implementation of <see cref="IJsonHttpClient"/> to use.</param>
        public HalClient(
            IJsonHttpClient client)
            : this(client, _defaultFormatters)
        {
            
        }

        private HalClient(
            HalClient client,
            IEnumerable<IResource> current)
            : this(client._client, client._formatters.ToList())
        {
            _current = current;
        }

        /// <summary>
        /// Gets the instance of <see cref="System.Net.Http.HttpClient"/> used by the <see cref="HalClient"/>.
        /// </summary>
        public HttpClient HttpClient => _client.HttpClient;

        /// <summary>
        /// Returns the most recently navigated resource of the specified type. 
        /// </summary>
        /// <typeparam name="T">The type of the resource to return.</typeparam>
        /// <returns>The most recent navigated resource of the specified type.</returns>
        /// <exception cref="NoActiveResource" />
        public IResource<T> Item<T>() where T : class, new() => Convert<T>(Latest);

        /// <summary>
        /// Returns the list of embedded resources in the most recently navigated resource.
        /// </summary>
        /// <typeparam name="T">The type of the resource to return.</typeparam>
        /// <returns>The list of embedded resources in the most recently navigated resource.</returns>
        /// <exception cref="NoActiveResource" />
        public IEnumerable<IResource<T>> Items<T>() where T : class, new() => _current.Select(Convert<T>);

        /// <summary>
        /// Makes a HTTP GET request to the default URI and stores the returned resource.
        /// </summary>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        public IHalClient Root() => Root(string.Empty);

        internal Task<IHalClient> ExecuteRootAsync() => ExecuteRootAsync(string.Empty);

        /// <summary>
        /// Makes a HTTP GET request to the given URL and stores the returned resource.
        /// </summary>
        /// <param name="href">The URI to request.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        public IHalClient Root(string href) => Execute(href, uri => _client.GetAsync(uri));

        internal Task<IHalClient> ExecuteRootAsync(string href) => ExecuteAsync(href, uri => _client.GetAsync(uri));

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Get(string rel) => Get(rel, null, null);

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Get(string rel, string curie) => Get(rel, null, curie);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Get(string rel, object parameters) => Get(rel, parameters, null);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Get(string rel, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            var embedded = _current.FirstOrDefault(r => r.Embedded.Any(e => e.Rel == relationship));
            if (embedded != null)
            {
                var current = embedded.Embedded.Where(e => e.Rel == relationship);
                return new HalClient(this, current);
            }

            return BuildAndExecute(relationship, parameters, uri => _client.GetAsync(uri));
        }

        internal async Task<IHalClient> ExecuteGetAsync(string rel, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            var embedded = _current.FirstOrDefault(r => r.Embedded.Any(e => e.Rel == relationship));
            if (embedded != null)
            {
                var current = embedded.Embedded.Where(e => e.Rel == relationship);
                return new HalClient(this, current);
            }

            return await BuildAndExecuteAsync(relationship, parameters, uri => _client.GetAsync(uri));
        }

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Get(IResource resource, string rel) => Get(resource, rel, null, null);

        /// <summary>
        /// Navigates the given link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Get(IResource resource, string rel, string curie) => Get(resource, rel, null, curie);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Get(IResource resource, string rel, object parameters) => Get(resource, rel, parameters, null);

        /// <summary>
        /// Navigates the given templated link relation and stores the the returned resource(s).
        /// </summary>
        /// <param name="resource">The current <see cref="IResource"/>.</param>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Get(IResource resource, string rel, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            if (resource.Embedded.Any(e => e.Rel == relationship))
            {
                var current = resource.Embedded.Where(e => e.Rel == relationship);
                return new HalClient(this, current);
            }

            var link = resource.Links.FirstOrDefault(l => l.Rel == relationship);
            if (link == null)
                throw new FailedToResolveRelationship(relationship);

            return Execute(Construct(link, parameters), uri => _client.GetAsync(uri));
        }

        internal async Task<IHalClient> ExecuteGetAsync(IResource resource, string rel, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            if (resource.Embedded.Any(e => e.Rel == relationship))
            {
                var current = resource.Embedded.Where(e => e.Rel == relationship);
                return new HalClient(this, current);
            }

            var link = resource.Links.FirstOrDefault(l => l.Rel == relationship);
            if (link == null)
                throw new FailedToResolveRelationship(relationship);

            return await ExecuteAsync(Construct(link, parameters), uri => _client.GetAsync(uri));
        }

        /// <summary>
        /// Makes a HTTP POST request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Post(string rel, object value) => Post(rel, value, null, null);

        /// <summary>
        /// Makes a HTTP POST request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Post(string rel, object value, string curie) => Post(rel, value, null, curie);

        /// <summary>
        /// Makes a HTTP POST request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Post(string rel, object value, object parameters) => Post(rel, value, parameters, null);

        /// <summary>
        /// Makes a HTTP POST request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to POST.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Post(string rel, object value, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            return BuildAndExecute(relationship, parameters, uri => _client.PostAsync(uri, value));
        }

        internal Task<IHalClient> ExecutePostAsync(string rel, object value, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            return BuildAndExecuteAsync(relationship, parameters, uri => _client.PostAsync(uri, value));
        }

        /// <summary>
        /// Makes a HTTP PUT request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Put(string rel, object value) => Put(rel, value, null, null);

        /// <summary>
        /// Makes a HTTP PUT request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Put(string rel, object value, string curie) => Put(rel, value, null, curie);

        /// <summary>
        /// Makes a HTTP PUT request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Put(string rel, object value, object parameters) => Put(rel, value, parameters, null);

        /// <summary>
        /// Makes a HTTP PUT request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PUT.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Put(string rel, object value, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            return BuildAndExecute(relationship, parameters, uri => _client.PutAsync(uri, value));
        }

        internal Task<IHalClient> ExecutePutAsync(string rel, object value, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            return BuildAndExecuteAsync(relationship, parameters, uri => _client.PutAsync(uri, value));
        }

        /// <summary>
        /// Makes a HTTP PATCH request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Patch(string rel, object value) => Patch(rel, value, null, null);

        /// <summary>
        /// Makes a HTTP PATCH request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Patch(string rel, object value, string curie) => Patch(rel, value, null, curie);

        /// <summary>
        /// Makes a HTTP PATCH request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Patch(string rel, object value, object parameters) => Patch(rel, value, parameters, null);

        /// <summary>
        /// Makes a HTTP PATCH request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="value">The payload to PATCH.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Patch(string rel, object value, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            return BuildAndExecute(relationship, parameters, uri => _client.PatchAsync(uri, value));
        }

        internal Task<IHalClient> ExecutePatchAsync(string rel, object value, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            return BuildAndExecuteAsync(relationship, parameters, uri => _client.PatchAsync(uri, value));
        }

        /// <summary>
        /// Makes a HTTP DELETE request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Delete(string rel) => Delete(rel, null, null);

        /// <summary>
        /// Makes a HTTP DELETE request to the given link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The link relation to follow.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        public IHalClient Delete(string rel, string curie) => Delete(rel, null, curie);

        /// <summary>
        /// Makes a HTTP DELETE request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Delete(string rel, object parameters) => Delete(rel, parameters, null);

        /// <summary>
        /// Makes a HTTP DELETE request to the given templated link relation on the most recently navigated resource.
        /// </summary>
        /// <param name="rel">The templated link relation to follow.</param>
        /// <param name="parameters">An anonymous object containing the template parameters to apply.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>The updated <see cref="IHalClient"/>.</returns>
        /// <exception cref="FailedToResolveRelationship" />
        /// <exception cref="TemplateParametersAreRequired" />
        public IHalClient Delete(string rel, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            return BuildAndExecute(relationship, parameters, uri => _client.DeleteAsync(uri));
        }

        internal Task<IHalClient> ExecuteDeleteAsync(string rel, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            return BuildAndExecuteAsync(relationship, parameters, uri => _client.DeleteAsync(uri));
        }

        /// <summary>
        /// Determines whether the most recently navigated resource contains the given link relation.
        /// </summary>
        /// <param name="rel">The link relation to look for.</param>
        /// <returns>Whether or not the link relation exists.</returns>
        public bool Has(string rel) => Has(rel, null);

        /// <summary>
        /// Determines whether the most recently navigated resource contains the given link relation.
        /// </summary>
        /// <param name="rel">The link relation to look for.</param>
        /// <param name="curie">The curie of the link relation.</param>
        /// <returns>Whether or not the link relation exists.</returns>
        public bool Has(string rel, string curie)
        {
            var relationship = Relationship(rel, curie);

            return
                _current.Any(r => r.Embedded.Any(e => e.Rel == relationship))
                || _current.Any(r => r.Links.Any(l => l.Rel == relationship));
        }

        private IHalClient BuildAndExecute(string relationship, object parameters, Func<string, Task<HttpResponseMessage>> command)
        {
            var resource = _current.FirstOrDefault(r => r.Links.Any(l => l.Rel == relationship));
            if (resource == null)
                throw new FailedToResolveRelationship(relationship);

            var link = resource.Links.FirstOrDefault(l => l.Rel == relationship);
            return Execute(Construct(link, parameters), command);
        }

        internal Task<IHalClient> BuildAndExecuteAsync(string relationship, object parameters, Func<string, Task<HttpResponseMessage>> command)
        {
            var resource = _current.FirstOrDefault(r => r.Links.Any(l => l.Rel == relationship));
            if (resource == null)
                throw new FailedToResolveRelationship(relationship);

            var link = resource.Links.FirstOrDefault(l => l.Rel == relationship);
            return ExecuteAsync(Construct(link, parameters), command);
        }

        private IHalClient Execute(string uri, Func<string, Task<HttpResponseMessage>> command)
        {
            var result = command(uri).Result;

            return Process(result);
        }

        internal async Task<IHalClient> ExecuteAsync(string uri, Func<string, Task<HttpResponseMessage>> command)
        {
            var result = await command(uri);

            return Process(result);
        }

        private IHalClient Process(HttpResponseMessage result)
        {
            AssertSuccessfulStatusCode(result);

            var current =
                new[]
                {
                    result.Content == null
                        ? new Resource()
                        : result.Content.ReadAsAsync<Resource>(_formatters).Result
                };

            return new HalClient(this, current);
        }

        private static string Construct(ILink link, object parameters)
        {
            if (!link.Templated)
                return link.Href;

            if (parameters == null)
                throw new TemplateParametersAreRequired(link);

            var template = new UriTemplate(link.Href, caseInsensitiveParameterNames: true);
            template.AddParameters(parameters);
            return template.Resolve();
        }

        private static IResource<T> Convert<T>(IResource resource)
            where T : class, new() =>
                new Resource<T>
                {
                    Rel = resource.Rel,
                    Href = resource.Href,
                    Name = resource.Name,
                    Data = resource.Data<T>(),
                    Links = resource.Links,
                    Embedded = resource.Embedded
                };

        private static void AssertSuccessfulStatusCode(HttpResponseMessage result)
        {
            if (!result.IsSuccessStatusCode)
                throw new HttpRequestFailed(result.StatusCode);
        }

        private IResource Latest
        {
            get
            {
                if (_current == null || !_current.Any())
                    throw new NoActiveResource();
                return _current.Last();
            }
        }

        private static string Relationship(string rel, string curie) => curie == null ? rel : $"{curie}:{rel}";
    }
}