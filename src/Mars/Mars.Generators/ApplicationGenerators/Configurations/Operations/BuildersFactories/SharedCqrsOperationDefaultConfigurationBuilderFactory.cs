namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuildersFactories;

public class SharedCqrsOperationDefaultConfigurationBuilderFactory
{
    public static CqrsOperationsSharedConfiguration Construct()
    {
        return new()
        {
            BusinessLogicNamespaceBasePath = new("{{assembly_name}}.Application.{{feature_name}}.{{function_name}}"),
            EndpointsNamespaceBasePath = new("{{assembly_name}}.Endpoints.{{entity_name}}Endpoints"),
            FeatureNameConfigurationBuilder = new("{{entity_name}}Feature")
        };
    }
}