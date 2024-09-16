using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace Mars.Api;

/// <summary>
/// Serialize ObjectId as string and deserialize string as ObjectId
/// </summary>
public class MongoObjectIdJsonConverter : JsonConverter<ObjectId>
{
    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new ObjectId(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}