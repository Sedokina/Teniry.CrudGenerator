### DateTimeOffset fields should be configured additionally

MongoDb stores DateTimeOffset fields in an own way, that can cause issues with serialization.
To avoid this, you should apply the configuration`.HasBsonRepresentation(BsonType.DateTime)` or
`.HasBsonRepresentation(BsonType.String)` to the fields with this type in the `IEntityTypeConfiguration`.

### Int ID

Currently create operation does not accept id field in the request body. Therefore, the id field is generated
automatically.
In MongoDB, the ID field is generated automatically by the database. However, it does not support automatic generation
of Int ID fields. Currently, the workaround is to use a custom value generator.
But it would fit only if you have one mongo db.

```csharp
/// <summary> 
/// Generator that creates Int Id for MongoDB.
/// </summary> 
public class MongoEfIntIdSequenceGenerator<T> : ValueGenerator<int> where T : class
{
    public override bool GeneratesTemporaryValues => false;

    public override int Next(EntityEntry entry)
    {
        var currInd = entry.Context.Set<T>().Count();

        return currInd + 1;
    }
}

// Register the generator in your DbContext
public class MongoDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ...
        modelBuilder.Entity<IntIdEntity>().Property(x => x.Id)
            .HasValueGenerator<MongoEfIntIdSequenceGenerator<IntIdEntity>>();
        // ...
    }
}
```