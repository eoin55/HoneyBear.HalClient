using System.Collections.Generic;
using HoneyBear.HalClient.Serialization;
using Newtonsoft.Json;

namespace HoneyBear.HalClient.Models
{
    [JsonConverter(typeof (HalResourceJsonConverter))]
    public interface IResource : IDictionary<string, object>, INode
    {
        IList<ILink> Links { get; }
        IList<IResource> Embedded { get; }
    }

    [JsonConverter(typeof (HalResourceJsonConverter))]
    public interface IResource<out T> : IResource
        where T : class, new()
    {
        T Data { get; }
    }
}