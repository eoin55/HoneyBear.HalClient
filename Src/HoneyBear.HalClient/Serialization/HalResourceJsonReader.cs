using System.Collections.Generic;
using HoneyBear.HalClient.Models;
using Newtonsoft.Json;

namespace HoneyBear.HalClient.Serialization
{
    internal static class HalResourceJsonReader
    {
        public static IResource ReadResource(JsonReader reader, JsonSerializer serializer)
        {
            SkipComments(reader);
            AssertNextTokenIsStartObject(reader);

            var resource = new Resource();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var propertyName = reader.Value.ToString();
                        ReadNextToken(reader);

                        switch (propertyName)
                        {
                            case "_links":
                                resource.Links = ReadLinks(reader, serializer);
                                break;
                            case "_embedded":
                                resource.Embedded = ReadEmbedded(reader, serializer);
                                break;
                            default:
                                resource[propertyName] = serializer.Deserialize(reader);
                                break;
                        }
                        continue;
                    case JsonToken.EndObject:
                        return resource;
                    case JsonToken.Comment:
                        continue;
                    default:
                        throw new JsonSerializationException($"Unexpected token encountered:{reader.TokenType}");
                }
            }

            throw new JsonSerializationException("Unexpected end of tokens.");
        }

        private static IList<ILink> ReadLinks(JsonReader reader, JsonSerializer serializer)
        {
            SkipComments(reader);
            AssertNextTokenIsStartObject(reader);

            var links = new List<ILink>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var rel = reader.Value.ToString();
                        ReadNextToken(reader);
                        links.AddRange(ReadLinks(reader, serializer, rel));
                        continue;
                    case JsonToken.Comment:
                        continue;
                    case JsonToken.EndObject:
                        return links;
                    default:
                        throw new JsonSerializationException($"Unexpected token encountered:{reader.TokenType}");
                }
            }

            throw new JsonSerializationException("Unexpected end of tokens.");
        }

        private static IEnumerable<Link> ReadLinks(JsonReader reader, JsonSerializer serializer, string rel)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var link = serializer.Deserialize<Link>(reader);
                    link.Rel = rel;
                    return new[] { link };
                case JsonToken.StartArray:
                    return serializer.Deserialize<Link[]>(reader);
                default:
                    throw new JsonSerializationException($"Unexpected token encountered:{reader.TokenType}");
            }
        }

        private static IList<IResource> ReadEmbedded(JsonReader reader, JsonSerializer serializer)
        {
            SkipComments(reader);
            AssertNextTokenIsStartObject(reader);

            var embedded = new List<IResource>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var rel = reader.Value.ToString();
                        ReadNextToken(reader);
                        embedded.AddRange(ReadEmbedded(reader, serializer, rel));
                        continue;
                    case JsonToken.Comment:
                        continue;
                    case JsonToken.EndObject:
                        return embedded;
                    default:
                        throw new JsonSerializationException($"Unexpected token encountered:{reader.TokenType}");
                }
            }

            throw new JsonSerializationException("Unexpected end of tokens.");
        }

        private static IEnumerable<IResource> ReadEmbedded(JsonReader reader, JsonSerializer serializer, string rel)
        {
            switch (reader.TokenType)
            {
                case JsonToken.StartObject:
                    var resource = serializer.Deserialize<IResource>(reader);
                    resource.Rel = rel;
                    return new[] { resource };
                case JsonToken.StartArray:
                    var resources = serializer.Deserialize<Resource[]>(reader);
                    foreach (var r in resources)
                        r.Rel = rel;
                    return resources;
                default:
                    throw new JsonSerializationException($"Unexpected token encountered:{reader.TokenType}");
            }
        }

        private static void SkipComments(JsonReader reader)
        {
            while (reader.TokenType == JsonToken.Comment)
                if (!reader.Read())
                    throw new JsonSerializationException("Unexpected end of tokens.");
        }

        private static void ReadNextToken(JsonReader reader)
        {
            if (!reader.Read())
                throw new JsonSerializationException("Unexpected end of tokens.");
        }

        private static void AssertNextTokenIsStartObject(JsonReader reader)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException($"Unexpected token encountered:{reader.TokenType}");
        }
    }
}