using ITech.CrudGenerator.Core.Configurations.Configurators;
using ITech.CrudGenerator.Core.Schemes.Entity;

namespace ITech.CrudGenerator.Core.Configurations.Shared;

internal record CqrsOperationsSharedConfiguration
{
    public string BusinessLogicFeatureName { get; set; } = null!;
    public string BusinessLogicNamespaceForOperation { get; set; } = null!;
    public string EndpointsNamespaceForFeature { get; set; } = null!;

    public CqrsOperationsSharedConfiguration(NameConfigurator businessLogicFeatureName,
        PutBusinessLogicIntoNamespaceConfigurator businessLogicNamespaceForOperation,
        PutEndpointsIntoNamespaceConfigurator endpointsNamespaceForFeature,
        EntityScheme entityScheme,
        string operationName,
        string operationGroup)
    {
        BusinessLogicFeatureName = businessLogicFeatureName.GetName(entityScheme.EntityName, operationName);
        BusinessLogicNamespaceForOperation = businessLogicNamespaceForOperation
            .GetNamespacePath(
                entityScheme.ContainingAssembly,
                BusinessLogicFeatureName,
                operationGroup,
                entityScheme.EntityName);
        EndpointsNamespaceForFeature = endpointsNamespaceForFeature
            .GetNamespacePath(entityScheme.EntityName, entityScheme.ContainingAssembly);
    }
}