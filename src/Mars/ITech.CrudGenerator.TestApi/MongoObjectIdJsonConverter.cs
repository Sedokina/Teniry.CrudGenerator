using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace ITech.CrudGenerator.TestApi;

/// <summary>
///     Serialize ObjectId as string and deserialize string as ObjectId
/// </summary>
public class MongoObjectIdJsonConverter : JsonConverter<ObjectId> {
    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        var value = reader.GetString();
        try {
            return new(value);
        } catch (Exception e) {
            Console.WriteLine(e);

            throw new FailedToParseMongoObjectIdException($"Value \"{value}\" is not allowed for ObjectId");
        }
    }

    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToString());
    }
}

public class FailedToParseMongoObjectIdException : Exception {
    public FailedToParseMongoObjectIdException(string message) : base(message) { }
}