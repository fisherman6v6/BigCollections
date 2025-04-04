using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BigCollections
{
    public class BigListJsonConverter<T> : JsonConverter<BigList<T>>
    {
        public override BigList<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Not memory efficient. It is simply based on array deserialization. To be improved
            var items = JsonSerializer.Deserialize<T[]>(ref reader, options) ?? throw new JsonException("Unable to deserialize BigList.");

            var bigList = new BigList<T>(items);

            return bigList;
        }

        public override void Write(Utf8JsonWriter writer, BigList<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}