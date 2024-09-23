namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

internal class MinimalApiEndpointConfiguration : FileTemplateBasedOperationConfiguration
{
    public bool Generate { get; set; } = true;
    public string FunctionName { get; set; } = "";
    public string Route { get; set; } = "";
}