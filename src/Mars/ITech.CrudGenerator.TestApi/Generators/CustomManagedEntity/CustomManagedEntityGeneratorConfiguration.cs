using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.TestApi.Generators.CustomManagedEntity;

public class CustomManagedEntityGeneratorConfiguration : EntityGeneratorConfiguration<CustomManagedEntity>
{
    public CustomManagedEntityGeneratorConfiguration()
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
            RouteName = "/customizedManageEntityCreate"
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
            RouteName = "/customizedManageEntityDelete/{{entity_name}}/{{id_param_name}}"
        };

        UpdateOperation = new EntityGeneratorUpdateOperationConfiguration
        {
            // Generate = true,
            // Operation = "Upd",
            // OperationGroup = "UpddCurcy",
            CommandName = "CustomizedNameUpdateManageEntityCommand",
            HandlerName = "CustomizedNameUpdateManageEntityHandler",
            // GenerateEndpoint = true,
            ViewModelName = "CustomizedNameUpdateManageEntityViewModel",
            EndpointClassName = "CustomizedNameUpdateManageEntityEndpoint",
            EndpointFunctionName = "RunUpdateAsync",
            RouteName = "/customizedManageEntityUpdate/{{id_param_name}}"
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