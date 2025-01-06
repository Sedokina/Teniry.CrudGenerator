using System.Linq;
using System.Threading;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders.TypedBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;
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
        CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration> scheme,
        EndpointRouteConfigurationBuilder? getByIdEndpointRouteConfigurationBuilder,
        string? getByIdOperationName) : base(scheme)
    {
        _getByIdEndpointRouteConfigurationBuilder = getByIdEndpointRouteConfigurationBuilder;
        _getByIdOperationName = getByIdOperationName;
        _commandName = scheme.Configuration.Operation;
        _handlerName = scheme.Configuration.Handler;
        _dtoName = scheme.Configuration.Dto;
        _endpointClassName = scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator()
    {
        GenerateCommand();
        GenerateHandler();
        GenerateDto();
        if (Scheme.Configuration.Endpoint.Generate)
        {
            GenerateEndpoint();
        }
    }

    private void GenerateCommand()
    {
        var command = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _commandName)
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .WithXmlDoc($"Create {Scheme.EntityScheme.EntityTitle}",
                $"Returns id of created entity of type <see cref=\"{_dtoName}\" />");

        foreach (var property in EntityScheme.NotPrimaryKeys)
        {
            command.WithProperty(property.TypeName, property.PropertyName)
                .WithDefaultValue(property.DefaultValue);
        }

        WriteFile(_commandName, command.BuildAsString());
    }

    private void GenerateDto()
    {
        var dtoClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _dtoName)
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation);

        var constructorParameters = EntityScheme.PrimaryKeys
            .Select(x => new ParameterOfMethodBuilder(x.TypeName, x.PropertyNameAsMethodParameterName)).ToList();
        var constructor = new ConstructorBuilder([SyntaxKind.PublicKeyword], _dtoName)
            .WithParameters(constructorParameters);
        var constructorBody = new MethodBodyBuilder();
        foreach (var primaryKey in EntityScheme.PrimaryKeys)
        {
            dtoClass.WithProperty(primaryKey.TypeName, primaryKey.PropertyName);
            constructorBody.AssignVariable(primaryKey.PropertyName, primaryKey.PropertyNameAsMethodParameterName);
        }

        constructor.WithBody(constructorBody.Build());
        dtoClass.WithConstructor(constructor.Build());

        WriteFile(_dtoName, dtoClass.BuildAsString());
    }

    private void GenerateHandler()
    {
        var handlerClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _handlerName)
            .WithUsings([
                "ITech.Cqrs.Cqrs.Commands",
                Scheme.DbContextScheme.DbContextNamespace,
                EntityScheme.EntityNamespace,
                "Mapster",
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .Implements("ICommandHandler", _commandName, _dtoName)
            .WithPrivateField([SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword],
                Scheme.DbContextScheme.DbContextName, "_db");

        var constructor = new ConstructorBuilder([SyntaxKind.PublicKeyword], _handlerName)
            .WithParameters([new ParameterOfMethodBuilder(Scheme.DbContextScheme.DbContextName, "db")]);
        var constructorBody = new MethodBodyBuilder()
            .AssignVariable("_db", "db");

        constructor.WithBody(constructorBody.Build());

        var methodBuilder = new MethodBuilder([
                    SyntaxKind.PublicKeyword,
                    SyntaxKind.AsyncKeyword
                ], $"Task<{_dtoName}>", "HandleAsync")
            .WithParameters([
                new ParameterOfMethodBuilder(_commandName, "command"),
                new ParameterOfMethodBuilder(nameof(CancellationToken), "cancellation")
            ])
            .WithXmlInheritdoc();

        var constructorParams = EntityScheme.PrimaryKeys.GetAsMethodCallParameters("entity");
        var methodBodyBuilder = new MethodBodyBuilder()
            .InitVariableFromGenericMethodCall("entity", "command", "Adapt", [EntityScheme.EntityName.ToString()], [])
            .CallAsyncMethod("_db", "AddAsync", ["entity", "cancellation"])
            .CallAsyncMethod("_db", "SaveChangesAsync", ["cancellation"])
            .InitVariableFromConstructorCall("result", _dtoName, constructorParams)
            .ReturnVariable("result");

        methodBuilder.WithBody(methodBodyBuilder.Build());
        handlerClass.WithConstructor(constructor.Build());
        handlerClass.WithMethod(methodBuilder.Build());

        WriteFile(_handlerName, handlerClass.BuildAsString());
    }

    private void GenerateEndpoint()
    {
        var endpointClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.PartialKeyword
            ], _endpointClassName)
            .WithUsings([
                "Microsoft.AspNetCore.Mvc",
                "ITech.Cqrs.Cqrs.Commands",
                Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature);

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
            .WithXmlDoc(
                $"Create {Scheme.EntityScheme.EntityTitle}",
                201,
                $"New {Scheme.EntityScheme.EntityTitle} created");

        var methodBodyBuilder = new MethodBodyBuilder()
            .InitVariableFromGenericAsyncMethodCall("result", "commandDispatcher", "DispatchAsync",
                [_commandName, _dtoName],
                ["command", "cancellation"])
            .ReturnTypedResultCreated(GetByIdRoute(), "result");

        methodBuilder.WithBody(methodBodyBuilder.Build());
        endpointClass.WithMethod(methodBuilder.Build());

        WriteFile(_endpointClassName, endpointClass.BuildAsString());

        EndpointMap = new EndpointMap(EntityScheme.EntityTitle.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Post",
            Scheme.Configuration.Endpoint.Route,
            _endpointClassName,
            Scheme.Configuration.Endpoint.FunctionName);
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