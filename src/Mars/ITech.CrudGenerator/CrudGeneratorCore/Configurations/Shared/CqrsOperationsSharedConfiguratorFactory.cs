namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Shared;

internal class CqrsOperationsSharedConfiguratorFactory
{
    public CqrsOperationsSharedConfigurator Construct()
    {
        return new()
        {
            BusinessLogicFeatureName = new("{{entity_name}}Feature"),
            BusinessLogicNamespaceForOperation =
                new("{{entity_assembly_name}}.Application.{{business_logic_feature_name}}.{{operation_group}}"),
            EndpointsNamespaceForFeature = new("{{entity_assembly_name}}.Endpoints.{{entity_name}}Endpoints")
        };
    }
}