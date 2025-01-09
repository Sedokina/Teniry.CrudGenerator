using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Configurators;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Shared;

internal class CqrsOperationsSharedConfigurator
{
    public NameConfigurator BusinessLogicFeatureName { get; set; } = null!;
    public PutBusinessLogicIntoNamespaceConfigurator BusinessLogicNamespaceForOperation { get; set; } = null!;
    public PutEndpointsIntoNamespaceConfigurator EndpointsNamespaceForFeature { get; set; } = null!;
}