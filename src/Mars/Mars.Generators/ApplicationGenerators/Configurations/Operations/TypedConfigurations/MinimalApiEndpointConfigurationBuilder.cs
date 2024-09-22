using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;

public class MinimalApiEndpointConfigurationBuilder
{
    public string TemplatePath { get; set; } = null!;
    public NameConfiguration NameConfiguration { get; set; } = null!;

    public EndpointRouteConfiguration RouteConfiguration { get; set; } = null!;

    // TODO: remove direct usage of entity name, get it from entity
    public string GetName(EntityName entityName)
    {
        return NameConfiguration.GetName(entityName);
    }
}