using Teniry.CrudGenerator.Abstractions.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore.Extensions;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CurrencyGenerator;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomIds.GuidEntityGenerator;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomIds.IntIdEntityGenerator;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomManagedEntityGenerator;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomOperationNameEntityGenerator;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.ReadOnlyCustomizedEntityGenerator;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.SimpleEntityGenerator;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.SimpleTypeEntityGenerator;

namespace Teniry.CrudGenerator.SampleApi;

public class Mmb : DbContext {
    public Mmb() { }

    public Mmb(DbContextOptions<SampleMongoDb> options) : base(options) { }
}

[UseDbContext(DbContextDbProvider.Mongo)]
public class SampleMongoDb : Mmb {
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Country> Countries { get; set; }
    public SampleMongoDb() { }

    public SampleMongoDb(DbContextOptions<SampleMongoDb> options, IServiceProvider services) : base(options) { }

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
        modelBuilder.Entity<ReadOnlyCustomizedEntity>().ToCollection("readOnlyCustomizedEntities");
        modelBuilder.Entity<CustomOperationNameEntity>().ToCollection("customOperationNameEntities");
        modelBuilder.Entity<IntIdEntity>().ToCollection("intIdEntities");
        modelBuilder.Entity<IntIdEntity>().Property(x => x.Id)
            .HasValueGenerator<MongoEfIntIdSequenceGenerator<IntIdEntity>>();
        modelBuilder.Entity<GuidEntity>().ToCollection("guidEntities");
    }
}

public class MongoEfIntIdSequenceGenerator<T> : ValueGenerator<int> where T : class {
    public override bool GeneratesTemporaryValues => false;

    public override int Next(EntityEntry entry) {
        var currInd = entry.Context.Set<T>().Count();

        return currInd + 1;
    }
}