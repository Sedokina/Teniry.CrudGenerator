namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.GeneratorConfigurations.TypedConfigurations;

internal record MinimalApiEndpointConfiguration
{
    public string Name { get; set; } = "";
    public bool Generate { get; set; } = true;
    public string FunctionName { get; set; } = "";
    public string Route { get; set; } = "";
}