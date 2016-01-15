using System.Collections.Generic;
using HoneyBear.HalClient.Models;

namespace HoneyBear.HalClient
{
    public interface IHalClient
    {
        IResource<T> Item<T>() where T : class, new();
        IEnumerable<IResource<T>> Items<T>() where T : class, new();

        IHalClient Root();
        IHalClient Root(string href);

        IHalClient Get(string rel);
        IHalClient Get(string rel, string curie);
        IHalClient Get(string rel, object parameters);
        IHalClient Get(string rel, object parameters, string curie);

        IHalClient Post(string rel, object value);
        IHalClient Post(string rel, object value, string curie);
        IHalClient Post(string rel, object value, object parameters);
        IHalClient Post(string rel, object value, object parameters, string curie);

        IHalClient Put(string rel, object value);
        IHalClient Put(string rel, object value, string curie);
        IHalClient Put(string rel, object value, object parameters);
        IHalClient Put(string rel, object value, object parameters, string curie);

        IHalClient Delete(string rel);
        IHalClient Delete(string rel, string curie);
        IHalClient Delete(string rel, object parameters);
        IHalClient Delete(string rel, object parameters, string curie);

        bool Has(string rel);
        bool Has(string rel, string curie);
    }
}