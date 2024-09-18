using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;
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
        var properties = EntityScheme.NotPrimaryKeys.FormatAsProperties();
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
        var properties = EntityScheme.PrimaryKeys.FormatAsProperties();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorBody = EntityScheme.PrimaryKeys.FormatAsConstructorBody();
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
        var constructorParams = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters("entity.");

        var model = new
        {
            CommandName = _commandName,
            DtoName = _dtoName,
            HandlerName = _handlerName,
            CreatedDtoConstructorParams = constructorParams
        };

        WriteFile(templatePath, model, _handlerName);
    }

    private void GenerateEndpoint(string templatePath)
    {
        var parameters = EntityScheme.PrimaryKeys.GetAsMethodCallParameters("result.");
        var getEntityRoute = Configuration.FullConfiguration.GetByIdQueryGenerator.EndpointRouteConfiguration
            .GetRoute(EntityScheme.EntityName.ToString(), parameters);
        var interpolatedStringRoute = $"$\"{getEntityRoute}\"";

        var model = new
        {
            EndpointClassName = _endpointClassName,
            CommandName = _commandName,
            GetEntityRoute = interpolatedStringRoute,
            DtoName = _dtoName
        };

        WriteFile(templatePath, model, _endpointClassName);
        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            EndpointNamespace,
            "Post",
            Configuration.EndpointRouteConfiguration.GetRoute(EntityScheme.EntityName.ToString()),
            $"{_endpointClassName}.{Configuration.EndpointRouteConfiguration.FunctionName}");
    }
}