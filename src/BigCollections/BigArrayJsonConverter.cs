using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BigCollections
{
    public class BigArrayJsonConverter<T> : JsonConverter<BigArray<T>>
    {
        public override BigArray<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Not memory efficient. It is simply based on array deserialization. To be improved
            var items = JsonSerializer.Deserialize<T[]>(ref reader, options) ?? throw new JsonException("Unable to deserialize BigArray.");
            var bigArray = new BigArray<T>(items);

            return bigArray;
        }

        public override void Write(Utf8JsonWriter writer, BigArray<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}