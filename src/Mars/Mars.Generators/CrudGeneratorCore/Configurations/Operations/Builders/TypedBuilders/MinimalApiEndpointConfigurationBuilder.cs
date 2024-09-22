using Mars.Generators.CrudGeneratorCore.Schemes.Entity;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

internal class MinimalApiEndpointConfigurationBuilder
{
    public string TemplatePath { get; set; } = null!;
    public NameConfigurationBuilder NameConfigurationBuilder { get; set; } = null!;
    public NameConfigurationBuilder FunctionName { get; set; } = null!;
    public EndpointRouteConfigurationBuilder RouteConfigurationBuilder { get; set; } = null!;

    // TODO: remove direct usage of entity name, get it from entity
    public string GetName(EntityName entityName)
    {
        return NameConfigurationBuilder.GetName(entityName);
    }
}