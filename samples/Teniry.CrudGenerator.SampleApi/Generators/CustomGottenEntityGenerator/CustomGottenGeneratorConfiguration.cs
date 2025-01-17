using Teniry.CrudGenerator.Abstractions.Configuration;

namespace Teniry.CrudGenerator.SampleApi.Generators.CustomGottenEntityGenerator;

public class CustomGottenGeneratorConfiguration : EntityGeneratorConfiguration<CustomGottenEntity> {
    public CustomGottenGeneratorConfiguration() {
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
            OperationGroup = "CustomGottenEntityGetOperationCustomNs",
            QueryName = "CustomizedNameGetCustomEntityQuery",
            HandlerName = "CustomizedNameGetCustomEntityHandler",
            DtoName = "CustomizedNameGetCustomEntityDto",
            EndpointClassName = "CustomizedNameGetCustomEntityEndpoint",
            EndpointFunctionName = "RunGetAsync",
            RouteName = "/getCustomGottenEntityById/{{id_param_name}}"
        };
        GetListOperation = new() {
            OperationGroup = "CustomGottenEntityGetListOperationCustomNs",
            QueryName = "CustomizedNameGetCustomEntitiesListQuery",
            HandlerName = "CustomizedNameGetCustomEntitiesListHandler",
            DtoName = "CustomizedNameGetCustomEntitiesListDto",
            EndpointClassName = "CustomizedNameGetCustomEntitiesListEndpoint",
            EndpointFunctionName = "RunGetListAsync",
            RouteName = "/getAllCustomGottenEntitiesList"
        };
    }
}