using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;

internal class
    CreateCommandCrudGenerator : BaseOperationCrudGenerator<CqrsOperationWithReturnValueGeneratorConfiguration>
{
    private readonly EndpointRouteConfigurationBuilder? _getByIdEndpointRouteConfigurationBuilder;
    private readonly string? _getByIdOperationName;
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _endpointClassName;
    private readonly string _dtoName;

    public CreateCommandCrudGenerator(
        GeneratorExecutionContext context,
        CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration> scheme,
        EndpointRouteConfigurationBuilder? getByIdEndpointRouteConfigurationBuilder,
        string? getByIdOperationName) : base(context, scheme)
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
        if (Scheme.Configuration.Endpoint.Generate)
        {
            GenerateEndpoint();
        }
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

    private void GenerateEndpoint()
    {
        var endpointClass = new ClassBuilder(_endpointClassName)
            .WithUsings([
                "Microsoft.AspNetCore.Mvc",
                "ITech.Cqrs.Cqrs.Commands",
                Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature);

        var methodXmlDoc = @$"
/// <summary>
///     Create {Scheme.EntityScheme.EntityTitle}
/// </summary>
/// <response code=""201"">New {Scheme.EntityScheme.EntityTitle} created</response>
";
        var methodBuilder = new MethodBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.AsyncKeyword
            ], "Task<IResult>", Scheme.Configuration.Endpoint.FunctionName)
            .WithParameters([
                new ParameterOfMethodBuilder(_commandName, "command"),
                new ParameterOfMethodBuilder("ICommandDispatcher", "commandDispatcher"),
                new ParameterOfMethodBuilder("CancellationToken", "cancellation"),
            ])
            .WithProducesResponseTypeAttribute(201)
            .WithXmlDoc(methodXmlDoc);

        var methodBodyBuilder = new MethodBodyBuilder()
            .InitVariableFromGenericAsyncMethodCall("result", "commandDispatcher", "DispatchAsync",
                [_commandName, _dtoName],
                ["command", "cancellation"])
            .ReturnTypedResultCreated(GetByIdRoute(), "result");

        methodBuilder.WithBody(methodBodyBuilder.Build());
        endpointClass.WithMethod(methodBuilder.Build());

        WriteFile(_endpointClassName, endpointClass.BuildAsString());

        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Post",
            Scheme.Configuration.Endpoint.Route,
            $"{_endpointClassName}.{Scheme.Configuration.Endpoint.FunctionName}");
    }

    private string GetByIdRoute()
    {
        var parameters = EntityScheme.PrimaryKeys.GetAsMethodCallParameters("result.");
        if (_getByIdEndpointRouteConfigurationBuilder != null && _getByIdOperationName != null)
        {
            var getEntityRoute = _getByIdEndpointRouteConfigurationBuilder
                .GetRoute(EntityScheme.EntityName.ToString(), _getByIdOperationName, parameters);
            return getEntityRoute;
        }

        return "";
    }
}