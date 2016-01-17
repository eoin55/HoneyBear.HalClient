using System.Collections.Generic;
using HoneyBear.HalClient.Serialization;
using Newtonsoft.Json;

namespace HoneyBear.HalClient.Models
{
    /// <summary>
    /// Represents a generic HAL resource.
    /// </summary>
    [JsonConverter(typeof (HalResourceJsonConverter))]
    public interface IResource : IDictionary<string, object>, INode
    {
        /// <summary>
        /// The list of link relations.
        /// </summary>
        IList<ILink> Links { get; }

        /// <summary>
        /// The list of embedded resources.
        /// </summary>
        IList<IResource> Embedded { get; }
    }

    /// <summary>
    /// Represents a type-specific HAL resource.
    /// </summary>
    /// <typeparam name="T">The type of the resource.</typeparam>
    [JsonConverter(typeof (HalResourceJsonConverter))]
    public interface IResource<out T> : IResource
        where T : class, new()
    {
        /// <summary>
        /// The content of the resource.
        /// </summary>
        T Data { get; }
    }
}