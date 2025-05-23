using Teniry.CrudGenerator.Abstractions.Configuration;

namespace Teniry.CrudGenerator.SampleApi.CrudConfigurations.WriteOnlyCustomizedGenerator;

public class WriteOnlyCustomizedEntityGeneratorConfiguration : EntityGeneratorConfiguration<WriteOnlyCustomizedEntity> {
    public WriteOnlyCustomizedEntityGeneratorConfiguration() {
        CreateOperation = new() {
            OperationGroup = "ManagedEntityCreateOperationCustomNs",
            CommandName = "CustomizedNameCreateManagedEntityCommand",
            HandlerName = "CustomizedNameCreateManagedEntityHandler",
            DtoName = "CustomizedNameCreatedManagedEntityDto",
            EndpointClassName = "CustomizedNameCreateManagedEntityEndpoint",
            EndpointFunctionName = "RunCreateAsync",
            RouteName = "/customizedManagedEntityCreate"
        };

        DeleteOperation = new() {
            OperationGroup = "ManagedEntityDeleteOperationCustomNs",
            CommandName = "CustomizedNameDeleteManagedEntityCommand",
            HandlerName = "CustomizedNameDeleteManagedEntityHandler",
            EndpointClassName = "CustomizedNameDeleteManagedEntityEndpoint",
            EndpointFunctionName = "RunDeleteAsync",
            RouteName = "/customizedManagedEntityDelete/{{entity_name}}/{{id_param_name}}"
        };

        UpdateOperation = new() {
            OperationGroup = "ManagedEntityUpdateOperationCustomNs",
            CommandName = "CustomizedNameUpdateManagedEntityCommand",
            HandlerName = "CustomizedNameUpdateManagedEntityHandler",
            ViewModelName = "CustomizedNameUpdateManagedEntityViewModel",
            EndpointClassName = "CustomizedNameUpdateManagedEntityEndpoint",
            EndpointFunctionName = "RunUpdateAsync",
            RouteName = "/customizedManagedEntityUpdate/{{id_param_name}}"
        };

        PatchOperation = new() {
            OperationGroup = "ManagedEntityPatchOperationCustomNs",
            CommandName = "CustomizedNamePatchManagedEntityCommand",
            HandlerName = "CustomizedNamePatchManagedEntityHandler",
            ViewModelName = "CustomizedNamePatchManagedEntityViewModel",
            EndpointClassName = "CustomizedNamePatchManagedEntityEndpoint",
            EndpointFunctionName = "RunPatchAsync",
            RouteName = "/customizedManagedEntityPatch/{{id_param_name}}"
        };

        GetByIdOperation = new() {
            Generate = false
        };

        GetListOperation = new() {
            Generate = false
        };
    }
}