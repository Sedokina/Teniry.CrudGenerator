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
            EndpointClassName = "CretEndp",
            EndpointFunctionName = "juj",
            RouteName = "/cru/cre"
        };
        DeleteOperation = new EntityGeneratorDeleteOperationConfiguration
        {
            Generate = true,
            OperationType = "Del",
            OperationGroup = "DeleLCurrency",
            OperationName = "DeledCurencyComandl",
            HandlerName = "DelCurCo",
            GenerateEndpoint = true,
            EndpointClassName = "DdelEndpo",
            EndpointFunctionName = "dkd",
            RouteName = "/cur/de/{{entity_name}}/{{id_param_name}}"
        };
    }
}
//
// public class CountryGeneratorConfiguration : EntityGeneratorConfiguration<Country>
// {
// }