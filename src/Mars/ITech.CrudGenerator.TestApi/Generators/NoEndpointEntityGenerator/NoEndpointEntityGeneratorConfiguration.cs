using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.TestApi.Generators.NoEndpointEntityGenerator;

public class NoEndpointEntityGeneratorConfiguration : EntityGeneratorConfiguration<NoEndpointEntity> {
    public NoEndpointEntityGeneratorConfiguration() {
        GetByIdOperation = new() {
            GenerateEndpoint = false
        };
        GetListOperation = new() {
            GenerateEndpoint = false
        };
        CreateOperation = new() {
            GenerateEndpoint = false
        };
        DeleteOperation = new() {
            GenerateEndpoint = false
        };
        UpdateOperation = new() {
            GenerateEndpoint = false
        };
    }
}