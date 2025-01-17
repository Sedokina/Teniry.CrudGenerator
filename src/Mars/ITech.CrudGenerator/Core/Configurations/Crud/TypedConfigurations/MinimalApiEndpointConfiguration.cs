namespace ITech.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;

internal record MinimalApiEndpointConfiguration(string Name, bool Generate, string FunctionName, string Route) {
    public string Name { get; } = Name;
    public bool Generate { get; } = Generate;
    public string FunctionName { get; } = FunctionName;
    public string Route { get; } = Route;
}