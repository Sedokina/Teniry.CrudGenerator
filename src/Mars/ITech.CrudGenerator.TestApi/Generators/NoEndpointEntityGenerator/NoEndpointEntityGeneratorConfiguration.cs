using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.TestApi.Generators.NoEndpointEntityGenerator;

public class NoEndpointEntityGeneratorConfiguration : EntityGeneratorConfiguration<NoEndpointEntity>
{
    public NoEndpointEntityGeneratorConfiguration()
    {
        GetByIdOperation = new EntityGeneratorGetByIdOperationConfiguration
        {
            GenerateEndpoint = false
        };
        GetListOperation = new EntityGeneratorGetListOperationConfiguration
        {
            GenerateEndpoint = false
        };
        CreateOperation = new EntityGeneratorCreateOperationConfiguration
        {
            GenerateEndpoint = false
        };
        DeleteOperation = new EntityGeneratorDeleteOperationConfiguration()
        {
            GenerateEndpoint = false
        };
        UpdateOperation = new EntityGeneratorUpdateOperationConfiguration
        {
            GenerateEndpoint = false
        };
    }
}