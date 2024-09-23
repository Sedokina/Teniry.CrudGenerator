using Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity.Formatters;

namespace Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

internal class MinimalApiEndpointConfigurationBuilder
{
    public bool Generate { get; set; } = true;
    public FileTemplatePathConfigurationBuilder TemplatePath { get; set; } = null!;
    public NameConfigurationBuilder NameConfigurationBuilder { get; set; } = null!;
    public NameConfigurationBuilder FunctionName { get; set; } = null!;
    public EndpointRouteConfigurationBuilder RouteConfigurationBuilder { get; set; } = null!;

    public MinimalApiEndpointConfiguration Build(
        EntityScheme entityScheme,
        string templatesBasePath,
        string operationName)
    {
        var constructorParametersForRoute = entityScheme.PrimaryKeys.GetAsMethodCallParameters();
        return new()
        {
            Generate = Generate,
            TemplatePath = TemplatePath.GetPath(templatesBasePath, operationName),
            Name = NameConfigurationBuilder.GetName(entityScheme.EntityName, operationName),
            FunctionName = FunctionName.GetName(entityScheme.EntityName, operationName),
            Route = RouteConfigurationBuilder
                .GetRoute(entityScheme.EntityName.Name, operationName, constructorParametersForRoute)
        };
    }
}