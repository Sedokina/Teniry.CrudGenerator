using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.DbContextCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

internal class CreateCommandCrudGenerator : BaseCrudGenerator< CqrsWithReturnValueConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _endpointClassName;
    private readonly string _dtoName;

    public CreateCommandCrudGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        CqrsWithReturnValueConfiguration configuration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme) : base(context, symbol, configuration, entityScheme, dbContextScheme)
    {
        _commandName = entityScheme.Configuration.CreateCommand.Operation.GetName(EntityScheme.EntityName);
        _handlerName = entityScheme.Configuration.CreateCommand.Handler.GetName(EntityScheme.EntityName);
        _dtoName = entityScheme.Configuration.CreateCommand.Dto.GetName(EntityScheme.EntityName);
        _endpointClassName = entityScheme.Configuration.CreateCommand.Endpoint.GetName(EntityScheme.EntityName);
    }

    public override void RunGenerator()
    {
        GenerateCommand(EntityScheme.Configuration.CreateCommand.Operation.TemplatePath);
        GenerateHandler(EntityScheme.Configuration.CreateCommand.Handler.TemplatePath);
        GenerateDto(EntityScheme.Configuration.CreateCommand.Dto.TemplatePath);
        GenerateEndpoint(EntityScheme.Configuration.CreateCommand.Endpoint.TemplatePath);
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
        var getEntityRoute = EntityScheme.Configuration.GetByIdQueryGenerator.EndpointRouteConfiguration
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
            EntityScheme.Configuration.CreateCommand.Endpoint.RouteConfiguration
                .GetRoute(EntityScheme.EntityName.ToString()),
            $"{_endpointClassName}.{EntityScheme.Configuration.CreateCommand.Endpoint.RouteConfiguration.FunctionName}");
    }
}