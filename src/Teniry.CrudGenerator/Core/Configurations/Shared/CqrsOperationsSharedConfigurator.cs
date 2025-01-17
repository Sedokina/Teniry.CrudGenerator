using Teniry.CrudGenerator.Core.Configurations.Configurators;

namespace Teniry.CrudGenerator.Core.Configurations.Shared;

internal class CqrsOperationsSharedConfigurator {
    public NameConfigurator BusinessLogicFeatureName { get; set; } = null!;
    public PutBusinessLogicIntoNamespaceConfigurator BusinessLogicNamespaceForOperation { get; set; } = null!;
    public PutEndpointsIntoNamespaceConfigurator EndpointsNamespaceForFeature { get; set; } = null!;
}