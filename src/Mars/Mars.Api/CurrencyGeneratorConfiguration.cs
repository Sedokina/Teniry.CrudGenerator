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
        UpdateOperation = new EntityGeneratorUpdateOperationConfiguration
        {
            Generate = true,
            OperationType = "Upd",
            OperationGroup = "UpddCurcy",
            OperationName = "UpdaCurencyComandl",
            HandlerName = "UdpCurCo",
            GenerateEndpoint = true,
            EndpointClassName = "UddmkEndpo",
            EndpointFunctionName = "mmupd",
            RouteName = "/cur/udo"
        };
        GetByIdOperation = new EntityGeneratorGetByIdOperationConfiguration
        {
            Generate = false,
            OperationType = "GtBy",
            OperationGroup = "IdGetCurcy",
            OperationName = "GetCurencyQr",
            HandlerName = "GetCurQuerHa",
            DtoName = "GetNotGetDto",
            GenerateEndpoint = true,
            EndpointClassName = "GtteCru",
            EndpointFunctionName = "ggetb",
            RouteName = "/cry/ggbyid"
        };
    }
}
//
// public class CountryGeneratorConfiguration : EntityGeneratorConfiguration<Country>
// {
// }