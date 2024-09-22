using Mars.Generators.CrudGeneratorCore.Schemes.DbContext;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Mars.Api;

[UseDbContext(DbContextDbProvider.Mongo)]
public class MarsDb : DbContext
{
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Country> Countries { get; set; }

    public MarsDb(DbContextOptions<MarsDb> options, IServiceProvider services) : base(options)
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