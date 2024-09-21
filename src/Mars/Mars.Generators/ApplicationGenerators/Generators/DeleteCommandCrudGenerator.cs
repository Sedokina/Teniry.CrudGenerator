using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.DbContextCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

internal class DeleteCommandCrudGenerator : BaseCrudGenerator<BaseCommandGeneratorConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _endpointClassName;

    public DeleteCommandCrudGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        BaseCommandGeneratorConfiguration configuration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme) : base(context, symbol, configuration, entityScheme, dbContextScheme)
    {
        _commandName = entityScheme.Configuration.DeleteCommand.Operation.GetName(EntityScheme.EntityName);
        _handlerName = entityScheme.Configuration.DeleteCommand.Handler.GetName(EntityScheme.EntityName);
        _endpointClassName = entityScheme.Configuration.DeleteCommand.Endpoint.GetName(EntityScheme.EntityName);
    }

    public override void RunGenerator()
    {
        GenerateCommand(EntityScheme.Configuration.DeleteCommand.Operation.TemplatePath);
        GenerateHandler(EntityScheme.Configuration.DeleteCommand.Handler.TemplatePath);
        GenerateEndpoint(EntityScheme.Configuration.DeleteCommand.Endpoint.TemplatePath);
    }

    private void GenerateCommand(string templatePath)
    {
        var properties = EntityScheme.PrimaryKeys.FormatAsProperties();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorBody = EntityScheme.PrimaryKeys.FormatAsConstructorBody();
        var model = new
        {
            CommandName = _commandName,
            Properties = properties,
            ConstructorParameters = constructorParameters,
            ConstructorBody = constructorBody
        };
        WriteFile(templatePath, model, _commandName);
    }

    private void GenerateHandler(string templatePath)
    {
        var findParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters("command");
        var model = new
        {
            CommandName = _commandName,
            HandlerName = _handlerName,
            FindParameters = findParameters
        };

        WriteFile(templatePath, model, _handlerName);
    }

    private void GenerateEndpoint(string templatePath)
    {
        var routeParams = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters();

        var model = new
        {
            EndpointClassName = _endpointClassName,
            RouteParams = routeParams,
            CommandName = _commandName,
            CommandConstructorParameters = constructorParameters
        };

        WriteFile(templatePath, model, _endpointClassName);

        var constructorParametersForRoute = EntityScheme.PrimaryKeys.GetAsMethodCallParameters();
        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            EndpointNamespace,
            "Delete",
            EntityScheme.Configuration.DeleteCommand.Endpoint.RouteConfiguration
                .GetRoute(EntityScheme.EntityName.ToString(), constructorParametersForRoute),
            $"{_endpointClassName}.{EntityScheme.Configuration.DeleteCommand.Endpoint.RouteConfiguration.FunctionName}");
    }
}