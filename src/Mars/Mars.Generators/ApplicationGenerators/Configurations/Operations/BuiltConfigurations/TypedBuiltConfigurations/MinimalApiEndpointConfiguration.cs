using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

public class MinimalApiEndpointConfiguration : FileTemplateBasedOperationConfiguration
{
    public string FunctionName { get; set; }
    public EndpointRouteConfigurationBuilder RouteConfigurationBuilder { get; set; } = null!;
}