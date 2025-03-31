using Teniry.CrudGenerator.Abstractions.Configuration;

namespace Teniry.CrudGenerator.SampleApi.CrudConfigurations.ReadOnlyCustomizedEntityGenerator;

public class ReadOnlyCustomizedGeneratorConfiguration : EntityGeneratorConfiguration<ReadOnlyCustomizedEntity> {
    public ReadOnlyCustomizedGeneratorConfiguration() {
        DefaultSort = new("desc", x => x.Name);

        CreateOperation = new() {
            Generate = false
        };

        DeleteOperation = new() {
            Generate = false
        };

        UpdateOperation = new() {
            Generate = false
        };

        GetByIdOperation = new() {
            OperationGroup = "GetReadOnlyModelCustomNamespace",
            QueryName = "GetReadOnlyModelQuery",
            HandlerName = "GetReadOnlyModelHandler",
            DtoName = "ReadOnlyModelCustomDto",
            EndpointClassName = "GetReadOnlyModelCustomizedEndpoint",
            EndpointFunctionName = "RunGetAsync",
            RouteName = "/getCustomizedReadOnlyModelById/{{id_param_name}}"
        };
        GetListOperation = new() {
            OperationGroup = "GetReadOnlyModelsListCustomNamespace",
            QueryName = "GetReadOnlyModelsQuery",
            HandlerName = "GetReadOnlyModelsHandler",
            DtoName = "ReadOnlyModelsListCustomDto",
            EndpointClassName = "ReadOnlyModelsListCustomizedEndpoint",
            EndpointFunctionName = "RunGetListAsync",
            RouteName = "/getAllCustomizedReadOnlyEntities"
        };
    }
}