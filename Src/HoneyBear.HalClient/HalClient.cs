using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HoneyBear.HalClient.Http;
using HoneyBear.HalClient.Models;
using Tavis.UriTemplates;

namespace HoneyBear.HalClient
{
    public class HalClient : IHalClient
    {
        private readonly IJsonHttpClient _client;
        private IEnumerable<IResource> _current;

        public HalClient(HttpClient client)
        {
            _client = new JsonHttpClient(client);
        }

        public HalClient()
            : this(new HttpClient())
        {
            
        }

        internal HalClient(IJsonHttpClient client)
        {
            _client = client;
        }

        public HttpClient HttpClient => _client.HttpClient;

        public IResource<T> Item<T>() where T : class, new() => Convert<T>(Latest);

        public IEnumerable<IResource<T>> Items<T>() where T : class, new() => _current.Select(Convert<T>);

        public IHalClient Root() => Execute(string.Empty, uri => _client.GetAsync(uri));

        public IHalClient Root(string href) => Execute(href, uri => _client.GetAsync(uri));

        public IHalClient Get(string rel) => Get(rel, null, null);

        public IHalClient Get(string rel, string curie) => Get(rel, null, curie);

        public IHalClient Get(string rel, object parameters) => Get(rel, parameters, null);

        public IHalClient Get(string rel, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            var embedded = _current.FirstOrDefault(r => r.Embedded.Any(e => e.Rel == relationship));
            if (embedded != null)
            {
                _current = embedded.Embedded.Where(e => e.Rel == relationship);
                return this;
            }

            return BuildAndExecute(relationship, parameters, uri => _client.GetAsync(uri));
        }

        public IHalClient Post(string rel, object value) => Post(rel, value, null, null);

        public IHalClient Post(string rel, object value, string curie) => Post(rel, value, null, curie);

        public IHalClient Post(string rel, object value, object parameters) => Post(rel, value, parameters, null);

        public IHalClient Post(string rel, object value, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            return BuildAndExecute(relationship, parameters, uri => _client.PostAsync(uri, value));
        }

        public IHalClient Put(string rel, object value) => Put(rel, value, null, null);

        public IHalClient Put(string rel, object value, string curie) => Put(rel, value, null, curie);

        public IHalClient Put(string rel, object value, object parameters) => Put(rel, value, parameters, null);

        public IHalClient Put(string rel, object value, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            return BuildAndExecute(relationship, parameters, uri => _client.PutAsync(uri, value));
        }

        public IHalClient Delete(string rel) => Delete(rel, null, null);

        public IHalClient Delete(string rel, string curie) => Delete(rel, null, curie);

        public IHalClient Delete(string rel, object parameters) => Delete(rel, parameters, null);

        public IHalClient Delete(string rel, object parameters, string curie)
        {
            var relationship = Relationship(rel, curie);

            return BuildAndExecute(relationship, parameters, uri => _client.DeleteAsync(uri));
        }

        public bool Has(string rel) => Has(rel, null);

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

        private IHalClient Execute(string uri, Func<string, Task<HttpResponseMessage>> command)
        {
            var result = command(uri).Result;

            AssertSuccessfulStatusCode(result);

            _current = new[] {result.Content == null ? new Resource() : result.Content.ReadAsAsync<Resource>().Result};

            return this;
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
                throw new HttpRequestException($"HTTP request returned non-200 HTTP status code:{result.StatusCode}");
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