using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.TestApi.Generators.CurrencyGenerator;
using ITech.CrudGenerator.TestApi.Generators.CustomGottenEntityGenerator;
using ITech.CrudGenerator.TestApi.Generators.CustomIds.EntityIdNameGenerator;
using ITech.CrudGenerator.TestApi.Generators.CustomManagedEntityGenerator;
using ITech.CrudGenerator.TestApi.Generators.CustomOperationNameEntityGenerator;
using ITech.CrudGenerator.TestApi.Generators.IntIdEntityGenerator;
using ITech.CrudGenerator.TestApi.Generators.SimpleEntityGenerator;
using ITech.CrudGenerator.TestApi.Generators.SimpleTypeEntityGenerator;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore.Extensions;

namespace ITech.CrudGenerator.TestApi;

[UseDbContext(DbContextDbProvider.Mongo)]
public class TestMongoDb : DbContext
{
    public TestMongoDb()
    {
    }

    public TestMongoDb(DbContextOptions<TestMongoDb> options, IServiceProvider services) : base(options)
    {
    }

    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Country> Countries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Currency>().ToCollection("currencies");
        modelBuilder.Entity<Country>().ToCollection("countries");
        modelBuilder.Entity<Currency>().HasOne(x => x.Country)
            .WithMany(x => x.Currencies)
            .HasForeignKey(x => x.CountryId);

        modelBuilder.Entity<SimpleEntity>().ToCollection("simpleEntities");
        modelBuilder.Entity<SimpleTypeEntity>().ToCollection("simpleTypeEntities");
        modelBuilder.Entity<SimpleTypeEntity>().Property(x => x.LastSignInDate)
            .HasBsonRepresentation(BsonType.DateTime);
        modelBuilder.Entity<SimpleTypeEntity>().Property(x => x.Id)
            .HasElementName("_id");
        modelBuilder.Entity<CustomManagedEntity>().ToCollection("customManagedEntities");
        modelBuilder.Entity<CustomGottenEntity>().ToCollection("customGottenEntities");
        modelBuilder.Entity<CustomOperationNameEntity>().ToCollection("customOperationNameEntities");
        modelBuilder.Entity<IntIdEntity>().ToCollection("intIdEntities");
        modelBuilder.Entity<IntIdEntity>().Property(x => x.Id)
            .HasValueGenerator<MongoEfIntIdSequenceGenerator<IntIdEntity>>();
        modelBuilder.Entity<EntityIdName>().ToCollection("entityIdNames");
    }
}

public class MongoEfIntIdSequenceGenerator<T> : ValueGenerator<int> where T : class
{
    public override bool GeneratesTemporaryValues => false;

    public override int Next(EntityEntry entry)
    {
        var currInd = entry.Context.Set<T>().Count();

        return currInd + 1;
    }
}