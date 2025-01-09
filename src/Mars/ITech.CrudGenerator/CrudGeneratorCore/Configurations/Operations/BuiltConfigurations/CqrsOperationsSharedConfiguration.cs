using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;

internal record CqrsOperationsSharedConfiguration
{
    public string BusinessLogicFeatureName { get; set; } = null!;
    public string BusinessLogicNamespaceForOperation { get; set; } = null!;
    public string EndpointsNamespaceForFeature { get; set; } = null!;

    public CqrsOperationsSharedConfiguration(NameConfigurationBuilder businessLogicFeatureName,
        PutBusinessLogicIntoNamespaceConfigurationBuilder businessLogicNamespaceForOperation,
        PutEndpointsIntoNamespaceConfigurationBuilder endpointsNamespaceForFeature,
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