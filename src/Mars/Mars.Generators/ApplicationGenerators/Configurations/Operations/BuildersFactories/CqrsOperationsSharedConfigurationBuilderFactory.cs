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
                new("{{business_logic_assembly_name}}.Application.{{business_logic_feature_name}}.{{operation_name}}"),
            EndpointsNamespaceForFeature = new("{{endpoints_assembly_name}}.Endpoints.{{entity_name}}Endpoints")
        };
    }
}