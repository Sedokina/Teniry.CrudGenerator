using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class DeleteCommandCrudGenerator : BaseCrudGenerator<BaseCommandGeneratorConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _endpointClassName;

    public DeleteCommandCrudGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        BaseCommandGeneratorConfiguration configuration,
        EntityScheme entityScheme) : base(context, symbol, configuration, entityScheme)
    {
        _commandName = Configuration.CommandNameConfiguration.GetName(EntityScheme.EntityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(EntityScheme.EntityName);
        _endpointClassName = Configuration.EndpointNameConfiguration.GetName(EntityScheme.EntityName);
    }

    public override void RunGenerator()
    {
        GenerateCommand(Configuration.CommandTemplatePath);
        GenerateHandler(Configuration.HandlerTemplatePath);
        GenerateEndpoint(Configuration.EndpointTemplatePath);
    }

    private void GenerateCommand(string templatePath)
    {
        var primaryKeys = PropertiesExtractor.GetPrimaryKeysOfEntity(Symbol);
        var properties = primaryKeys.ToClassPropertiesString();
        var constructorParameters = primaryKeys.ToMethodParametersString();
        var constructorBody = primaryKeys.ToConstructorBodyString();
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
        var properties = PropertiesExtractor.GetPrimaryKeysOfEntity(Symbol).ToPropertiesNamesList("command");
        var model = new
        {
            CommandName = _commandName,
            HandlerName = _handlerName,
            FindProperties = string.Join(", ", properties)
        };

        WriteFile(templatePath, model, _handlerName);
    }

    private void GenerateEndpoint(string templatePath)
    {
        var primaryKeys = PropertiesExtractor.GetPrimaryKeysOfEntity(Symbol);
        var routeParams = primaryKeys.ToMethodParametersString();
        var constructorParams = primaryKeys.ToPropertiesNamesList();

        var model = new
        {
            EndpointClassName = _endpointClassName,
            RouteParams = routeParams,
            CommandName = _commandName,
            CommandConstructorParameters = string.Join(", ", constructorParams)
        };

        WriteFile(templatePath, model, _endpointClassName);

        EndpointMap = new EndpointMap(EntityScheme.EntityName,
            EndpointNamespace,
            "Delete",
            Configuration.EndpointRouteConfiguration.GetRoute(EntityScheme.EntityName, constructorParams),
            $"{_endpointClassName}.{Configuration.EndpointRouteConfiguration.FunctionName}");
    }
}