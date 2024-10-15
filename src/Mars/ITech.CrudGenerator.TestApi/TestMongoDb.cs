using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.TestApi.Generators.CurrencyGenerator;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace ITech.CrudGenerator.TestApi;

[UseDbContext(DbContextDbProvider.Mongo)]
public class TestMongoDb : DbContext
{
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Country> Countries { get; set; }

    public TestMongoDb()
    {
    }

    public TestMongoDb(DbContextOptions<TestMongoDb> options, IServiceProvider services) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Currency>().ToCollection("currencies");
        modelBuilder.Entity<Country>().ToCollection("countries");
        modelBuilder.Entity<Currency>().HasOne(x => x.Country)
            .WithMany(x => x.Currencies)
            .HasForeignKey(x => x.CountryId);
    }
}