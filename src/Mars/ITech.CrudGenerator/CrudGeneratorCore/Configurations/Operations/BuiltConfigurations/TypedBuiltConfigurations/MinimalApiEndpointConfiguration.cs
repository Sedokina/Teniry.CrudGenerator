namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

internal record MinimalApiEndpointConfiguration
{
    public string Name { get; set; } = "";
    public bool Generate { get; set; } = true;
    public string FunctionName { get; set; } = "";
    public string Route { get; set; } = "";
}