using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.TestApi.Generators.CustomGottenEntity;

public class CustomGottenGeneratorConfiguration : EntityGeneratorConfiguration<CustomGottenEntity>
{
    public CustomGottenGeneratorConfiguration()
    {
        CreateOperation = new EntityGeneratorCreateOperationConfiguration()
        {
            Generate = false
            // // Generate = false,
            // // Operation = "Createll",
            // // OperationGroup = "CreateLCurrency",
            // CommandName = "CustomizedNameCreateManagedEntityCommand",
            // HandlerName = "CustomizedNameCreateManagedEntityHandler",
            // DtoName = "CustomizedNameCreatedManagedEntityDto",
            // // GenerateEndpoint = true,
            // EndpointClassName = "CustomizedNameCreateManagedEntityEndpoint",
            // EndpointFunctionName = "RunCreateAsync",
            // RouteName = "/customizedManagedEntityCreate"
        };

        DeleteOperation = new EntityGeneratorDeleteOperationConfiguration()
        {
            Generate = false
            // // Generate = true,
            // // Operation = "Del",
            // // OperationGroup = "DeleLCurrency",
            // CommandName = "CustomizedNameDeleteManagedEntityCommand",
            // HandlerName = "CustomizedNameDeleteManagedEntityHandler",
            // // GenerateEndpoint = true,
            // EndpointClassName = "CustomizedNameDeleteManagedEntityEndpoint",
            // EndpointFunctionName = "RunDeleteAsync",
            // RouteName = "/customizedManagedEntityDelete/{{entity_name}}/{{id_param_name}}"
        };

        UpdateOperation = new EntityGeneratorUpdateOperationConfiguration
        {
            Generate = false
            // // Generate = true,
            // // Operation = "Upd",
            // // OperationGroup = "UpddCurcy",
            // CommandName = "CustomizedNameUpdateManagedEntityCommand",
            // HandlerName = "CustomizedNameUpdateManagedEntityHandler",
            // // GenerateEndpoint = true,
            // ViewModelName = "CustomizedNameUpdateManagedEntityViewModel",
            // EndpointClassName = "CustomizedNameUpdateManagedEntityEndpoint",
            // EndpointFunctionName = "RunUpdateAsync",
            // RouteName = "/customizedManagedEntityUpdate/{{id_param_name}}"
        };

        GetByIdOperation = new EntityGeneratorGetByIdOperationConfiguration()
        {
            // Generate = false
        };
        GetListOperation = new EntityGeneratorGetListOperationConfiguration()
        {
            // Generate = false
        };
    }
}