using Teniry.CrudGenerator.Abstractions.Configuration;

namespace Teniry.CrudGenerator.SampleApi.CrudConfigurations.NoEndpointEntityGenerator;

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
        PatchOperation = new() {
            GenerateEndpoint = false
        };
    }
}