using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
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

            foreach (var property in dataType.GetTypeInfo().DeclaredProperties)
            {
                var propertyName = property.Name;
                var attribute = property.GetCustomAttributes<JsonPropertyAttribute>().FirstOrDefault();
                if (!string.IsNullOrEmpty(attribute?.PropertyName))
                    propertyName = attribute.PropertyName;

                var pair = source.FirstOrDefault(p => p.Key.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
                if (pair.Key == null)
                    continue;

                var propertyType = property.PropertyType;

                object value;
                var complex = pair.Value as JObject;
                var array = pair.Value as JArray;

                if (complex != null)
                    value = complex.ToObject(propertyType);
                else if (array != null)
                    value = array.ToObject(propertyType);
                else if (pair.Value != null)
                    value = TypeDescriptor.GetConverter(propertyType).ConvertFromInvariantString(pair.Value.ToString());
                else
                    value = null;

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
