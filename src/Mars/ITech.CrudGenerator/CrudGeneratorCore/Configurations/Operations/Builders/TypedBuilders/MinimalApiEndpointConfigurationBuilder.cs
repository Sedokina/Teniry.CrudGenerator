using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;

internal class MinimalApiEndpointConfigurationBuilder
{
    public bool Generate { get; set; } = true;
    public NameConfigurationBuilder ClassName { get; set; } = null!;
    public NameConfigurationBuilder FunctionName { get; set; } = null!;
    public EndpointRouteConfigurationBuilder RouteConfigurationBuilder { get; set; } = null!;

    public MinimalApiEndpointConfiguration Build(
        EntityScheme entityScheme,
        string operationName)
    {
        var constructorParametersForRoute = entityScheme.PrimaryKeys.GetAsMethodCallArguments();
        return new()
        {
            Generate = Generate,
            Name = ClassName.GetName(entityScheme.EntityName, operationName),
            FunctionName = FunctionName.GetName(entityScheme.EntityName, operationName),
            Route = RouteConfigurationBuilder
                .GetRoute(entityScheme.EntityName.Name, operationName, constructorParametersForRoute)
        };
    }
}