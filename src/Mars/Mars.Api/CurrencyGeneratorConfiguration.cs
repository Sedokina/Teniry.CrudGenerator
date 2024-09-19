using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Api;

[GenerateCrud]
public class CurrencyGeneratorConfiguration : EntityGeneratorConfiguration<Currency>
{
    public CurrencyGeneratorConfiguration()
    {
        Title = new EntityTitle("Curr", "Currs");
        DefaultSort = new EntityGeneratorDefaultSort<Currency>("asc", x => x.Name);
    }
}