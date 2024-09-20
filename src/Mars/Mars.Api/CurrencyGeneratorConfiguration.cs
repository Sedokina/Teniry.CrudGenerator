using Mars.Generators.ApplicationGenerators.Core;

namespace Mars.Api;

[GenerateCrud]
public class CurrencyGeneratorConfiguration : EntityGeneratorConfiguration<Currency>
{
    protected CurrencyGeneratorConfiguration()
    {
        // Title = "Curr";
        // TitlePlural = "Currsive";
        // DefaultSort = new EntityGeneratorDefaultSort<Currency>("desc", x => x.Name);
    }

    // public CurrencyGeneratorConfiguration(string someParam)
    // {
    //     Title = someParam;
    // }
}