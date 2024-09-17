using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class CreateCommandCrudGenerator : BaseCrudGenerator<CommandWithReturnTypeGeneratorConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _endpointClassName;
    private readonly string _dtoName;

    public CreateCommandCrudGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        CommandWithReturnTypeGeneratorConfiguration configuration,
        EntityScheme entityScheme) : base(context, symbol, configuration, entityScheme)
    {
        _commandName = Configuration.CommandNameConfiguration.GetName(EntityScheme.EntityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(EntityScheme.EntityName);
        _dtoName = Configuration.DtoNameConfiguration.GetName(EntityScheme.EntityName);
        _endpointClassName = Configuration.EndpointNameConfiguration.GetName(EntityScheme.EntityName);
    }

    public override void RunGenerator()
    {
        GenerateCommand(Configuration.CommandTemplatePath);
        GenerateDto(Configuration.DtoTemplatePath);
        GenerateHandler(Configuration.HandlerTemplatePath);
        GenerateEndpoint(Configuration.EndpointTemplatePath);
    }

    private void GenerateCommand(string templatePath)
    {
        var properties = PropertiesExtractor.GetAllPropertiesOfEntity(Symbol, true);
        var model = new
        {
            CommandName = _commandName,
            DtoName = _dtoName,
            Properties = properties
        };

        WriteFile(templatePath, model, _commandName);
    }

    private void GenerateDto(string templatePath)
    {
        var primaryKeys = PropertiesExtractor.GetPrimaryKeysOfEntity(Symbol);
        var properties = primaryKeys.ToClassPropertiesString();
        var constructorParameters = primaryKeys.ToMethodParametersString();
        var constructorBody = primaryKeys.ToConstructorBodyString();
        var model = new
        {
            DtoName = _dtoName,
            Properties = properties,
            ConstructorParameters = constructorParameters,
            ConstructorBody = constructorBody
        };
        WriteFile(templatePath, model, _dtoName);
    }

    private void GenerateHandler(string templatePath)
    {
        var constructorParams = PropertiesExtractor.GetPrimaryKeysOfEntity(Symbol).ToPropertiesNamesList("entity.");

        var model = new
        {
            CommandName = _commandName,
            DtoName = _dtoName,
            HandlerName = _handlerName,
            CreatedDtoConstructorParams = string.Join(", ", constructorParams)
        };

        WriteFile(templatePath, model, _handlerName);
    }

    private void GenerateEndpoint(string templatePath)
    {
        var props = PropertiesExtractor.GetPrimaryKeysOfEntity(Symbol).ToPropertiesNamesList("result.");
        var getEntityRoute = Configuration.FullConfiguration.GetByIdQueryGenerator.EndpointRouteConfiguration
            .GetRoute(EntityScheme.EntityName, props);
        var interpolatedStringRoute = $"$\"{getEntityRoute}\"";

        var model = new
        {
            EndpointClassName = _endpointClassName,
            CommandName = _commandName,
            GetEntityRoute = interpolatedStringRoute,
            DtoName = _dtoName
        };

        WriteFile(templatePath, model, _endpointClassName);
        EndpointMap = new EndpointMap(EntityScheme.EntityName,
            EndpointNamespace,
            "Post",
            Configuration.EndpointRouteConfiguration.GetRoute(EntityScheme.EntityName),
            $"{_endpointClassName}.{Configuration.EndpointRouteConfiguration.FunctionName}");
    }
}