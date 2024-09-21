using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations;

public class CqrsOperationsSharedConfiguration
{
    public PutBusinessLogicIntoNamespaceConfiguration BusinessLogicNamespaceBasePath { get; set; } = null!;
    public PutEndpointsIntoNamespaceConfiguration EndpointsNamespaceBasePath { get; set; } = null!;
    public NameConfiguration FeatureNameConfiguration { get; set; } = null!;
}