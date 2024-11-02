using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.TestApi.Generators.CustomizedManageEntity;

public class CustomizedManageEntityGeneratorConfiguration : EntityGeneratorConfiguration<CustomizedManageEntity>
{
    public CustomizedManageEntityGeneratorConfiguration()
    {
        CreateOperation = new EntityGeneratorCreateOperationConfiguration()
        {
            // Generate = false,
            // Operation = "Createll",
            // OperationGroup = "CreateLCurrency",
            CommandName = "CustomizedNameCreateManageEntityCommand",
            HandlerName = "CustomizedNameCreateManageEntityHandler",
            DtoName = "CustomizedNameCreatedManageEntityDto",
            // GenerateEndpoint = true,
            EndpointClassName = "CustomizedNameCreateManageEntityEndpoint",
            EndpointFunctionName = "RunCreateAsync",
            // RouteName = "/cru/cre"
        };

        DeleteOperation = new EntityGeneratorDeleteOperationConfiguration()
        {
            // Generate = true,
            // Operation = "Del",
            // OperationGroup = "DeleLCurrency",
            CommandName = "CustomizedNameDeleteManageEntityCommand",
            HandlerName = "CustomizedNameDeleteManageEntityHandler",
            // GenerateEndpoint = true,
            EndpointClassName = "CustomizedNameDeleteManageEntityEndpoint",
            EndpointFunctionName = "RunDeleteAsync",
            // RouteName = "/cur/de/{{entity_name}}/{{id_param_name}}"
        };

        UpdateOperation = new EntityGeneratorUpdateOperationConfiguration
        {
            // Generate = true,
            // Operation = "Upd",
            // OperationGroup = "UpddCurcy",
            CommandName = "CustomizedNameUpdateManageEntityCommand",
            HandlerName = "CustomizedNameUpdateManageEntityHandler",
            // ViewModelName = "JjUp", <<--- остановилась тут
            // GenerateEndpoint = true,
            // EndpointClassName = "UddmkEndpo",
            // EndpointFunctionName = "mmupd",
            // RouteName = "/cur/udo"
        };

        GetByIdOperation = new EntityGeneratorGetByIdOperationConfiguration()
        {
            Generate = false
        };
        GetListOperation = new EntityGeneratorGetListOperationConfiguration()
        {
            Generate = false
        };
    }
}