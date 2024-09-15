using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Mars.Api;

public class MarsDb : DbContext
{
    public DbSet<Todo> Todos { get; set; }
    public DbSet<Currency> Currencies { get; set; }

    public MarsDb(DbContextOptions<MarsDb> options, IServiceProvider services) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Currency>().ToCollection("currencies");
        modelBuilder.Entity<Currency>().Property(x => x._id).HasBsonRepresentation(BsonType.ObjectId);
    }
}