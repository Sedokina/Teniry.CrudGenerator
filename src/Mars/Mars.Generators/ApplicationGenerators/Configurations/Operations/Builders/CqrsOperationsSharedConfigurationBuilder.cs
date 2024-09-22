using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;

internal class CqrsOperationsSharedConfigurationBuilder
{
    public NameConfigurationBuilder BusinessLogicFeatureName { get; set; } = null!;
    public PutBusinessLogicIntoNamespaceConfigurationBuilder BusinessLogicNamespaceForOperation { get; set; } = null!;
    public PutEndpointsIntoNamespaceConfigurationBuilder EndpointsNamespaceForFeature { get; set; } = null!;

    public CqrsOperationsSharedConfiguration Build(EntityScheme entityScheme, string operationName)
    {
        var businessLogicFeatureName = BusinessLogicFeatureName.GetName(entityScheme.EntityName);
        return new CqrsOperationsSharedConfiguration
        {
            BusinessLogicFeatureName = businessLogicFeatureName,
            BusinessLogicNamespaceForOperation = BusinessLogicNamespaceForOperation
                .GetNamespacePath(
                    entityScheme.ContainingAssembly,
                    businessLogicFeatureName,
                    operationName,
                    entityScheme.EntityName),
            EndpointsNamespaceForFeature = EndpointsNamespaceForFeature
                .GetNamespacePath(entityScheme.EntityName, entityScheme.ContainingAssembly)
        };
    }
}