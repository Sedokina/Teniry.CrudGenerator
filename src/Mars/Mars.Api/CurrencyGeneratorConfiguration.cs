using Mars.Generators.ApplicationGenerators.Core;

namespace Mars.Api;

public class CurrencyGeneratorConfiguration : EntityGeneratorConfiguration<Currency>
{
    protected CurrencyGeneratorConfiguration()
    {
        Title = "Curr";
        TitlePlural = "Currsive";
        DefaultSort = new EntityGeneratorDefaultSort<Currency>("asc", x => x.Name);
    }
}

public class CountryGeneratorConfiguration : EntityGeneratorConfiguration<Country>
{
}