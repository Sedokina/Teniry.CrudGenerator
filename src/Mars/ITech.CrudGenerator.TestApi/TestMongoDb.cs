using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.TestApi.Generators.CurrencyGenerator;
using ITech.CrudGenerator.TestApi.Generators.CustomManagedEntity;
using ITech.CrudGenerator.TestApi.Generators.SimpleEntityGenerator;
using ITech.CrudGenerator.TestApi.Generators.SimpleTypeDefaultSortEntityGenerator;
using ITech.CrudGenerator.TestApi.Generators.SimpleTypeEntityGenerator;
using Microsoft.EntityFrameworkCore;
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
        modelBuilder.Entity<SimpleTypeDefaultSortEntity>().ToCollection("simpleTypeDefaultSortEntities");
        modelBuilder.Entity<CustomManagedEntity>().ToCollection("customizedManageEntities");
    }
}