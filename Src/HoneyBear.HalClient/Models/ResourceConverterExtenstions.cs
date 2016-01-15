using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HoneyBear.HalClient.Models
{
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

        public static IEnumerable<T> Data<T>(this IEnumerable<IResource<T>> source)
            where T : class, new()
            => source.Select(s => s.Data);
    }
}