namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;

public class CqrsOperationsSharedConfiguration
{
    public string BusinessLogicFeatureName { get; set; } = null!;
    public string BusinessLogicNamespaceForOperation { get; set; } = null!;
    public string EndpointsNamespaceForFeature { get; set; } = null!;
}