using Mars.Generators.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.CrudGeneratorCore.OperationsGenerators.Core;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.CrudGeneratorCore.OperationsGenerators;

internal class CreateCommandCrudGenerator : BaseOperationCrudGenerator<CqrsOperationWithReturnValueGeneratorConfiguration>
{
    private readonly EndpointRouteConfigurationBuilder _getByIdEndpointRouteConfigurationBuilder;
    private readonly string _getByIdOperationName;
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _endpointClassName;
    private readonly string _dtoName;

    public CreateCommandCrudGenerator(
        GeneratorExecutionContext context,
        CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration> scheme,
        EndpointRouteConfigurationBuilder getByIdEndpointRouteConfigurationBuilder,
        string getByIdOperationName) : base(context, scheme)
    {
        _getByIdEndpointRouteConfigurationBuilder = getByIdEndpointRouteConfigurationBuilder;
        _getByIdOperationName = getByIdOperationName;
        _commandName = scheme.Configuration.Operation.Name;
        _handlerName = scheme.Configuration.Handler.Name;
        _dtoName = scheme.Configuration.Dto.Name;
        _endpointClassName = scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator()
    {
        GenerateCommand(Scheme.Configuration.Operation.TemplatePath);
        GenerateHandler(Scheme.Configuration.Handler.TemplatePath);
        GenerateDto(Scheme.Configuration.Dto.TemplatePath);
        GenerateEndpoint(Scheme.Configuration.Endpoint.TemplatePath);
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
        var getEntityRoute = _getByIdEndpointRouteConfigurationBuilder
            .GetRoute(EntityScheme.EntityName.ToString(), _getByIdOperationName, parameters);
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
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Post",
            Scheme.Configuration.Endpoint.Route,
            $"{_endpointClassName}.{Scheme.Configuration.Endpoint.FunctionName}");
    }
}