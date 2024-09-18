using Mars.Generators.ApplicationGenerators.Core;

namespace Mars.Generators.ApplicationGenerators;

public class DbContextScheme
{
    public string DbContextNamespace { get; set; }
    public string DbContextName { get; set; }
    public DbContextDbProvider Provider { get; set; }

    public DbContextScheme(string dbContextNamespace, string dbContextName, DbContextDbProvider provider)
    {
        DbContextNamespace = dbContextNamespace;
        DbContextName = dbContextName;
        Provider = provider;
    }
}