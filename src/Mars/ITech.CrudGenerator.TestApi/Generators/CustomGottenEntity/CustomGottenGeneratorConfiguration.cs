using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.TestApi.Generators.CustomGottenEntity;

public class CustomGottenGeneratorConfiguration : EntityGeneratorConfiguration<CustomGottenEntity>
{
    public CustomGottenGeneratorConfiguration()
    {
        DefaultSort = new EntityGeneratorDefaultSort<CustomGottenEntity>("desc", x => x.Name);

        CreateOperation = new EntityGeneratorCreateOperationConfiguration
        {
            Generate = false
        };

        DeleteOperation = new EntityGeneratorDeleteOperationConfiguration
        {
            Generate = false
        };

        UpdateOperation = new EntityGeneratorUpdateOperationConfiguration
        {
            Generate = false
        };

        GetByIdOperation = new EntityGeneratorGetByIdOperationConfiguration
        {
            // Operation = "Upd",
            OperationGroup = "CustomGottenEntityGetOperationCustomNs",
            QueryName = "CustomizedNameGetCustomEntityQuery",
            HandlerName = "CustomizedNameGetCustomEntityHandler",
            DtoName = "CustomizedNameGetCustomEntityDto",
            // GenerateEndpoint = true
            EndpointClassName = "CustomizedNameGetCustomEntityEndpoint",
            EndpointFunctionName = "RunGetAsync",
            RouteName = "/getCustomGottenEntityById/{{id_param_name}}",
        };
        GetListOperation = new EntityGeneratorGetListOperationConfiguration
        {
            // Operation = "Upd",
            OperationGroup = "CustomGottenEntityGetListOperationCustomNs",
            QueryName = "CustomizedNameGetCustomEntitiesListQuery",
            HandlerName = "CustomizedNameGetCustomEntitiesListHandler",
            DtoName = "CustomizedNameGetCustomEntitiesListDto",
            // GenerateEndpoint = true
            EndpointClassName = "CustomizedNameGetCustomEntitiesListEndpoint",
            EndpointFunctionName = "RunGetListAsync",
            RouteName = "/getAllCustomGottenEntitiesList",
        };
    }
}