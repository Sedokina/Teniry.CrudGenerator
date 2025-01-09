namespace ITech.CrudGenerator.Core.Configurations.Configurators;

internal class MinimalApiEndpointConfigurator
{
    public bool Generate { get; set; } = true;
    public NameConfigurator ClassName { get; set; } = null!;
    public NameConfigurator FunctionName { get; set; } = null!;
    public EndpointRouteConfigurator RouteConfigurator { get; set; } = null!;
}