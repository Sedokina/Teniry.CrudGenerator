using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Mars.Api;

public class MarsDb : DbContext
{
    public DbSet<Todo> Todos { get; set; }
    public DbSet<Currency> Currencies { get; set; }

    public MarsDb(DbContextOptions<MarsDb> options, IServiceProvider services) : base(options)
    {
        // var a = this.Currencies.ProjectToType<Todo>().ToListAsync()
    }
}