using ITech.CrudGenerator.Abstractions;
using ITech.CrudGenerator.Abstractions.Configuration;

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
            Operation = "Createll",
            OperationGroup = "CreateLCurrency",
            CommandName = "CreatellCurencyComandl",
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
            Operation = "Del",
            OperationGroup = "DeleLCurrency",
            CommandName = "DeledCurencyComandl",
            HandlerName = "DelCurCo",
            GenerateEndpoint = true,
            EndpointClassName = "DdelEndpo",
            EndpointFunctionName = "dkd",
            RouteName = "/cur/de/{{entity_name}}/{{id_param_name}}"
        };
        UpdateOperation = new EntityGeneratorUpdateOperationConfiguration
        {
            Generate = true,
            Operation = "Upd",
            OperationGroup = "UpddCurcy",
            CommandName = "UpdaCurencyComandl",
            HandlerName = "UdpCurCo",
            ViewModelName = "JjUp",
            GenerateEndpoint = true,
            EndpointClassName = "UddmkEndpo",
            EndpointFunctionName = "mmupd",
            RouteName = "/cur/udo"
        };
        GetByIdOperation = new EntityGeneratorGetByIdOperationConfiguration
        {
            Generate = true,
            Operation = "GtBy",
            OperationGroup = "IdGetCurcy",
            QueryName = "GetCurencyQr",
            HandlerName = "GetCurQuerHa",
            DtoName = "GetNotGetDto",
            GenerateEndpoint = true,
            EndpointClassName = "GtteCru",
            EndpointFunctionName = "ggetb",
            RouteName = "/cry/ggbyid"
        };
        GetListOperation = new EntityGeneratorGetListOperationConfiguration
        {
            Generate = true,
            Operation = "GtLi",
            OperationGroup = "ListCuries",
            QueryName = "GetCurieQr",
            HandlerName = "GetCurLisQuerHa",
            DtoName = "AllCurLi",
            ListItemDtoName = "SpecCurDo",
            FilterName = "AllcUrFilt",
            GenerateEndpoint = true,
            EndpointClassName = "CurLisGe",
            EndpointFunctionName = "lisgget",
            RouteName = "/cry/all/go"
        };
    }
}

public class CountryGeneratorConfiguration : EntityGeneratorConfiguration<Country>
{
}