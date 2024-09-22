namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

internal class MinimalApiEndpointConfigurationBuilder
{
    public string TemplatePath { get; set; } = null!;
    public NameConfigurationBuilder NameConfigurationBuilder { get; set; } = null!;
    public NameConfigurationBuilder FunctionName { get; set; } = null!;
    public EndpointRouteConfigurationBuilder RouteConfigurationBuilder { get; set; } = null!;
}