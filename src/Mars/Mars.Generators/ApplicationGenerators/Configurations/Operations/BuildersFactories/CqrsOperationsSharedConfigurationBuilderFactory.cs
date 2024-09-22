using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuildersFactories;

internal class CqrsOperationsSharedConfigurationBuilderFactory
{
    public static CqrsOperationsSharedConfigurationBuilder Construct()
    {
        return new()
        {
            BusinessLogicFeatureName = new("{{entity_name}}Feature"),
            BusinessLogicNamespaceForOperation =
                new("{{entity_assembly_name}}.Application.{{business_logic_feature_name}}.{{operation_name}}"),
            EndpointsNamespaceForFeature = new("{{entity_assembly_name}}.Endpoints.{{entity_name}}Endpoints")
        };
    }
}