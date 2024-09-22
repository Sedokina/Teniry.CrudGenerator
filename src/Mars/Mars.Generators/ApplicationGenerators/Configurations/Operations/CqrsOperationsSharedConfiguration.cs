using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations;

public class CqrsOperationsSharedConfiguration
{
    public PutBusinessLogicIntoNamespaceConfigurationBuilder BusinessLogicNamespaceBasePath { get; set; } = null!;
    public PutEndpointsIntoNamespaceConfigurationBuilder EndpointsNamespaceBasePath { get; set; } = null!;
    public NameConfigurationBuilder FeatureNameConfigurationBuilder { get; set; } = null!;
}