namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;

internal record CqrsOperationsSharedConfiguration
{
    public string BusinessLogicFeatureName { get; set; } = null!;
    public string BusinessLogicNamespaceForOperation { get; set; } = null!;
    public string EndpointsNamespaceForFeature { get; set; } = null!;
}