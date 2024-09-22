using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

public class MinimalApiEndpointConfiguration : FileTemplateBasedOperationConfiguration
{
    public EndpointRouteConfigurationBuilder RouteConfigurationBuilder { get; set; } = null!;
}