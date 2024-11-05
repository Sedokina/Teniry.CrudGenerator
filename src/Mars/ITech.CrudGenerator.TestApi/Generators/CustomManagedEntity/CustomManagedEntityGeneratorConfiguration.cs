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
            CommandName = "CustomizedNameCreateManagedEntityCommand",
            HandlerName = "CustomizedNameCreateManagedEntityHandler",
            DtoName = "CustomizedNameCreatedManagedEntityDto",
            // GenerateEndpoint = true,
            EndpointClassName = "CustomizedNameCreateManagedEntityEndpoint",
            EndpointFunctionName = "RunCreateAsync",
            RouteName = "/customizedManagedEntityCreate"
        };

        DeleteOperation = new EntityGeneratorDeleteOperationConfiguration()
        {
            // Generate = true,
            // Operation = "Del",
            // OperationGroup = "DeleLCurrency",
            CommandName = "CustomizedNameDeleteManagedEntityCommand",
            HandlerName = "CustomizedNameDeleteManagedEntityHandler",
            // GenerateEndpoint = true,
            EndpointClassName = "CustomizedNameDeleteManagedEntityEndpoint",
            EndpointFunctionName = "RunDeleteAsync",
            RouteName = "/customizedManagedEntityDelete/{{entity_name}}/{{id_param_name}}"
        };

        UpdateOperation = new EntityGeneratorUpdateOperationConfiguration
        {
            // Generate = true,
            // Operation = "Upd",
            // OperationGroup = "UpddCurcy",
            CommandName = "CustomizedNameUpdateManagedEntityCommand",
            HandlerName = "CustomizedNameUpdateManagedEntityHandler",
            // GenerateEndpoint = true,
            ViewModelName = "CustomizedNameUpdateManagedEntityViewModel",
            EndpointClassName = "CustomizedNameUpdateManagedEntityEndpoint",
            EndpointFunctionName = "RunUpdateAsync",
            RouteName = "/customizedManagedEntityUpdate/{{id_param_name}}"
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