using MongoDB.Bson;

namespace ITech.CrudGenerator.TestApi.Generators.CurrencyGenerator;

public class Currency
{
    // string with [BsonRepresentation(BsonType.ObjectId)] or ObjectId 
    public ObjectId _id { get; set; }

    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public string Symbol { get; set; } = "";
    public int? MyNumber { get; set; }
    public DateTime Dt1 { get; set; }
    public DateTimeOffset Dt3 { get; set; }
    public ObjectId? CountryId { get; set; }
    public Country? Country { get; set; }
    public bool IsCheck { get; set; }
}

// [GenerateCrud]
public class Country
{
    public ObjectId Id { get; set; }
    public string Name { get; set; } = "";
    public ICollection<Currency> Currencies { get; set; } = [];
}