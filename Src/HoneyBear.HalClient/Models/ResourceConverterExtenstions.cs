using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HoneyBear.HalClient.Models
{
    /// <summary>
    /// Contains a set of IResource extensions.
    /// </summary>
    public static class ResourceConverterExtenstions
    {
        internal static T Data<T>(this IResource source)
            where T : class, new()
        {
            var data = new T();
            var dataType = typeof(T);

            foreach (var property in dataType.GetProperties())
            {
                var pair = source.FirstOrDefault(p => p.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
                if (pair.Key == null)
                    continue;

                var propertyType = dataType.GetProperty(property.Name).PropertyType;

                var complex = pair.Value as JObject;
                var value =
                    complex != null
                        ? complex.ToObject(propertyType)
                        : TypeDescriptor.GetConverter(propertyType).ConvertFromInvariantString(pair.Value.ToString());

                property.SetValue(data, value, null);
            }

            return data;
        }

        /// <summary>
        /// Deserialises a list of resources into a given type.
        /// </summary>
        /// <param name="source">The list of resources.</param>
        /// <typeparam name="T">The type to deserialise the resources into.</typeparam>
        /// <returns>A list of deserialised POCOs.</returns>
        public static IEnumerable<T> Data<T>(this IEnumerable<IResource<T>> source)
            where T : class, new()
            => source.Select(s => s.Data);
    }
}