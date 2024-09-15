using Mars.Generators.ApplicationGenerators.Core;

namespace Mars.Api;

[GenerateCrud]
public class Currency
{
    // string with [BsonRepresentation(BsonType.ObjectId)] or ObjectId 
    public string _id { get; set; }

    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string Symbol { get; set; } = "";
    public int? MyNumber { get; set; }
    public DateTime Dt1 { get; set; }
    public DateTimeOffset Dt3 { get; set; }
}