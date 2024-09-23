using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;

internal class CqrsOperationsSharedConfigurationBuilder
{
    public NameConfigurationBuilder BusinessLogicFeatureName { get; set; } = null!;
    public PutBusinessLogicIntoNamespaceConfigurationBuilder BusinessLogicNamespaceForOperation { get; set; } = null!;
    public PutEndpointsIntoNamespaceConfigurationBuilder EndpointsNamespaceForFeature { get; set; } = null!;

    public CqrsOperationsSharedConfiguration Build(
        EntityScheme entityScheme,
        string operationName,
        string operationGroup)
    {
        var businessLogicFeatureName = BusinessLogicFeatureName.GetName(entityScheme.EntityName, operationName);
        return new CqrsOperationsSharedConfiguration
        {
            BusinessLogicFeatureName = businessLogicFeatureName,
            BusinessLogicNamespaceForOperation = BusinessLogicNamespaceForOperation
                .GetNamespacePath(
                    entityScheme.ContainingAssembly,
                    businessLogicFeatureName,
                    operationGroup,
                    entityScheme.EntityName),
            EndpointsNamespaceForFeature = EndpointsNamespaceForFeature
                .GetNamespacePath(entityScheme.EntityName, entityScheme.ContainingAssembly)
        };
    }
}