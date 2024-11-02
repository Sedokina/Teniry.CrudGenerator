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
            // EndpointClassName = "CretEndp",
            // EndpointFunctionName = "juj",
            // RouteName = "/cru/cre"
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