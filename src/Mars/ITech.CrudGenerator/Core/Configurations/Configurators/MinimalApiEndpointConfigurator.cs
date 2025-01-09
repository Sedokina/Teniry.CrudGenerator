using ITech.CrudGenerator.Core.Configurations.Crud.TypedConfigurations;
using ITech.CrudGenerator.Core.Schemes.Entity;
using ITech.CrudGenerator.Core.Schemes.Entity.Formatters;

namespace ITech.CrudGenerator.Core.Configurations.Configurators;

internal class MinimalApiEndpointConfigurator
{
    public bool Generate { get; set; } = true;
    public NameConfigurator ClassName { get; set; } = null!;
    public NameConfigurator FunctionName { get; set; } = null!;
    public EndpointRouteConfigurator RouteConfigurator { get; set; } = null!;

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
            Route = RouteConfigurator
                .GetRoute(entityScheme.EntityName.Name, operationName, constructorParametersForRoute)
        };
    }
}