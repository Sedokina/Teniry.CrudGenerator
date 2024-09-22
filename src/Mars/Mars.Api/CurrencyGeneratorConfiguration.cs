using Mars.Generators.CrudGeneratorCore.ConfigurationsReceiver;

namespace Mars.Api;

public class CurrencyGeneratorConfiguration : EntityGeneratorConfiguration<Currency>
{
    protected CurrencyGeneratorConfiguration()
    {
        Title = "Curr";
        TitlePlural = "Currsive";
        DefaultSort = new EntityGeneratorDefaultSort<Currency>("asc", x => x.Name);
        CreateOperation = new EntityGeneratorCreateOperationConfiguration
        {
            Generate = true,
            OperationType = "Createll",
            OperationGroup = "CreateLCurrency",
            OperationName = "CreatellCurencyComandl",
            HandlerName = "CrCurHandlerDd",
            DtoName = "CurnCyDtOo",
            GenerateEndpoint = true,
            EndpointName = "/cru/cre"
        };
    }
}
//
// public class CountryGeneratorConfiguration : EntityGeneratorConfiguration<Country>
// {
// }