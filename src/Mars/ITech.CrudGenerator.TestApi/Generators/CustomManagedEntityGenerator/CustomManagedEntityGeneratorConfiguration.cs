using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.TestApi.Generators.CustomManagedEntityGenerator;

public class CustomManagedEntityGeneratorConfiguration : EntityGeneratorConfiguration<CustomManagedEntity>
{
    public CustomManagedEntityGeneratorConfiguration()
    {
        CreateOperation = new EntityGeneratorCreateOperationConfiguration
        {
            OperationGroup = "ManagedEntityCreateOperationCustomNs",
            CommandName = "CustomizedNameCreateManagedEntityCommand",
            HandlerName = "CustomizedNameCreateManagedEntityHandler",
            DtoName = "CustomizedNameCreatedManagedEntityDto",
            EndpointClassName = "CustomizedNameCreateManagedEntityEndpoint",
            EndpointFunctionName = "RunCreateAsync",
            RouteName = "/customizedManagedEntityCreate"
        };

        DeleteOperation = new EntityGeneratorDeleteOperationConfiguration()
        {
            OperationGroup = "ManagedEntityDeleteOperationCustomNs",
            CommandName = "CustomizedNameDeleteManagedEntityCommand",
            HandlerName = "CustomizedNameDeleteManagedEntityHandler",
            EndpointClassName = "CustomizedNameDeleteManagedEntityEndpoint",
            EndpointFunctionName = "RunDeleteAsync",
            RouteName = "/customizedManagedEntityDelete/{{entity_name}}/{{id_param_name}}"
        };

        UpdateOperation = new EntityGeneratorUpdateOperationConfiguration
        {
            OperationGroup = "ManagedEntityUpdateOperationCustomNs",
            CommandName = "CustomizedNameUpdateManagedEntityCommand",
            HandlerName = "CustomizedNameUpdateManagedEntityHandler",
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