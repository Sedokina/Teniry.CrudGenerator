using Teniry.CrudGenerator.Abstractions.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore.Extensions;
using Teniry.CrudGenerator.SampleApi.Generators.CurrencyGenerator;
using Teniry.CrudGenerator.SampleApi.Generators.CustomGottenEntityGenerator;
using Teniry.CrudGenerator.SampleApi.Generators.CustomIds.EntityIdNameGenerator;
using Teniry.CrudGenerator.SampleApi.Generators.CustomManagedEntityGenerator;
using Teniry.CrudGenerator.SampleApi.Generators.CustomOperationNameEntityGenerator;
using Teniry.CrudGenerator.SampleApi.Generators.IntIdEntityGenerator;
using Teniry.CrudGenerator.SampleApi.Generators.SimpleEntityGenerator;
using Teniry.CrudGenerator.SampleApi.Generators.SimpleTypeEntityGenerator;

namespace Teniry.CrudGenerator.SampleApi;

public class Mmb : DbContext {
    public Mmb() { }

    public Mmb(DbContextOptions<TestMongoDb> options) : base(options) { }
}

[UseDbContext(DbContextDbProvider.Mongo)]
public class TestMongoDb : Mmb {
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Country> Countries { get; set; }
    public TestMongoDb() { }

    public TestMongoDb(DbContextOptions<TestMongoDb> options, IServiceProvider services) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
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

public class MongoEfIntIdSequenceGenerator<T> : ValueGenerator<int> where T : class {
    public override bool GeneratesTemporaryValues => false;

    public override int Next(EntityEntry entry) {
        var currInd = entry.Context.Set<T>().Count();

        return currInd + 1;
    }
}