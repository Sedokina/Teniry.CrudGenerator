using System.Linq;
using System.Threading;
using Teniry.CrudGenerator.Core.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Teniry.CrudGenerator.Core.Configurations.Configurators;
using Teniry.CrudGenerator.Core.Configurations.Crud;
using Teniry.CrudGenerator.Core.Generators.Core;
using Teniry.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders;
using Teniry.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders.Models;
using static Teniry.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders.SimpleSyntaxFactory;

namespace Teniry.CrudGenerator.Core.Generators;

internal class CreateCommandCrudGenerator
    : BaseOperationCrudGenerator<CqrsOperationWithReturnValueGeneratorConfiguration> {
    private readonly string _commandName;
    private readonly string _dtoName;
    private readonly string _endpointClassName;
    private readonly EndpointRouteConfigurator? _getByIdEndpointRouteConfigurationBuilder;
    private readonly string? _getByIdOperationName;
    private readonly string _handlerName;

    public CreateCommandCrudGenerator(
        CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration> scheme,
        EndpointRouteConfigurator? getByIdEndpointRouteConfigurationBuilder,
        string? getByIdOperationName
    ) : base(scheme) {
        _getByIdEndpointRouteConfigurationBuilder = getByIdEndpointRouteConfigurationBuilder;
        _getByIdOperationName = getByIdOperationName;
        _commandName = scheme.Configuration.Operation;
        _handlerName = scheme.Configuration.Handler;
        _dtoName = scheme.Configuration.Dto;
        _endpointClassName = scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator() {
        GenerateCommand();
        GenerateHandler();
        GenerateDto();
        if (Scheme.Configuration.Endpoint.Generate) {
            GenerateEndpoint();
        }
    }

    private void GenerateCommand() {
        var command = new ClassBuilder(
                [
                    SyntaxKind.PublicKeyword,
                    SyntaxKind.PartialKeyword
                ],
                _commandName
            )
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .WithXmlDoc(
                $"Create {Scheme.EntityScheme.EntityTitle}",
                $"Returns id of created entity of type <see cref=\"{_dtoName}\" />"
            );

        foreach (var property in EntityScheme.NotPrimaryKeys) {
            command.WithProperty(property.TypeName, property.PropertyName)
                .WithDefaultValue(property.DefaultValue);
        }

        WriteFile(_commandName, command.BuildAsString());
    }

    private void GenerateDto() {
        var dtoClass = new ClassBuilder(
                [
                    SyntaxKind.PublicKeyword,
                    SyntaxKind.PartialKeyword
                ],
                _dtoName
            )
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation);

        var constructorParameters = EntityScheme.PrimaryKeys
            .Select(x => new ParameterOfMethodBuilder(x.TypeName, x.PropertyNameAsMethodParameterName)).ToList();
        var constructor = new ConstructorBuilder(_dtoName)
            .WithParameters(constructorParameters);
        var constructorBody = new BlockBuilder();
        foreach (var primaryKey in EntityScheme.PrimaryKeys) {
            dtoClass.WithProperty(primaryKey.TypeName, primaryKey.PropertyName);
            constructorBody.AssignVariable(primaryKey.PropertyName, primaryKey.PropertyNameAsMethodParameterName);
        }

        constructor.WithBody(constructorBody);
        dtoClass.WithConstructor(constructor.Build());

        WriteFile(_dtoName, dtoClass.BuildAsString());
    }

    private void GenerateHandler() {
        var handlerClass = new ClassBuilder(
                [
                    SyntaxKind.PublicKeyword,
                    SyntaxKind.PartialKeyword
                ],
                _handlerName
            )
            .WithUsings(
                [
                    "Teniry.Cqrs.Commands",
                    Scheme.DbContextScheme.DbContextNamespace,
                    EntityScheme.EntityNamespace,
                    "Mapster"
                ]
            )
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .Implements("ICommandHandler", _commandName, _dtoName)
            .WithPrivateField(
                [SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword],
                Scheme.DbContextScheme.DbContextName,
                "_db"
            );

        var constructor = new ConstructorBuilder(_handlerName)
            .WithParameters([new ParameterOfMethodBuilder(Scheme.DbContextScheme.DbContextName, "db")]);
        var constructorBody = new BlockBuilder()
            .AssignVariable("_db", "db");

        constructor.WithBody(constructorBody);

        var methodBuilder = new MethodBuilder(
                [
                    SyntaxKind.PublicKeyword,
                    SyntaxKind.AsyncKeyword
                ],
                $"Task<{_dtoName}>",
                "HandleAsync"
            )
            .WithParameters(
                [
                    new ParameterOfMethodBuilder(_commandName, "command"),
                    new ParameterOfMethodBuilder(nameof(CancellationToken), "cancellation")
                ]
            )
            .WithXmlInheritdoc();

        var constructorParams = EntityScheme.PrimaryKeys
            .Select(x => Property("entity", x.PropertyName))
            .ToList<ExpressionSyntax>();

        var methodBodyBuilder = new BlockBuilder()
            .InitVariable("entity", CallGenericMethod("command", "Adapt", [EntityScheme.EntityName.ToString()], []))
            .CallAsyncMethod("_db", "AddAsync", [Variable("entity"), Variable("cancellation")])
            .CallAsyncMethod("_db", "SaveChangesAsync", [Variable("cancellation")])
            .Return(CallConstructor(_dtoName, constructorParams));

        methodBuilder.WithBody(methodBodyBuilder);
        handlerClass.WithConstructor(constructor.Build());
        handlerClass.WithMethod(methodBuilder.Build());

        WriteFile(_handlerName, handlerClass.BuildAsString());
    }

    private void GenerateEndpoint() {
        var endpointClass = new ClassBuilder(
                [
                    SyntaxKind.PublicKeyword,
                    SyntaxKind.StaticKeyword,
                    SyntaxKind.PartialKeyword
                ],
                _endpointClassName
            )
            .WithUsings(
                [
                    "Microsoft.AspNetCore.Mvc",
                    "Teniry.Cqrs.Commands",
                    Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation
                ]
            )
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature);

        var methodBuilder = new MethodBuilder(
                [
                    SyntaxKind.PublicKeyword,
                    SyntaxKind.StaticKeyword,
                    SyntaxKind.AsyncKeyword
                ],
                "Task<IResult>",
                Scheme.Configuration.Endpoint.FunctionName
            )
            .WithParameters(
                [
                    new ParameterOfMethodBuilder(_commandName, "command"),
                    new ParameterOfMethodBuilder("ICommandDispatcher", "commandDispatcher"),
                    new ParameterOfMethodBuilder("CancellationToken", "cancellation")
                ]
            )
            .WithAttribute(new(201))
            .WithXmlDoc(
                $"Create {Scheme.EntityScheme.EntityTitle}",
                201,
                $"New {Scheme.EntityScheme.EntityTitle} created"
            );

        var methodBodyBuilder = new BlockBuilder()
            .InitVariable(
                "result",
                CallGenericAsyncMethod(
                    "commandDispatcher",
                    "DispatchAsync",
                    [_commandName, _dtoName],
                    [Variable("command"), Variable("cancellation")]
                )
            )
            .Return(CallMethod("TypedResults", "Created", [InterpolatedString(GetByIdRoute()), Variable("result")]));

        methodBuilder.WithBody(methodBodyBuilder);
        endpointClass.WithMethod(methodBuilder.Build());

        WriteFile(_endpointClassName, endpointClass.BuildAsString());

        EndpointMap = new(
            EntityScheme.EntityTitle.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Post",
            Scheme.Configuration.Endpoint.Route,
            _endpointClassName,
            Scheme.Configuration.Endpoint.FunctionName
        );
    }

    private string GetByIdRoute() {
        var parameters = EntityScheme.PrimaryKeys.GetAsMethodCallParameters("result.");
        if (_getByIdEndpointRouteConfigurationBuilder != null && _getByIdOperationName != null) {
            var getEntityRoute = _getByIdEndpointRouteConfigurationBuilder
                .GetRoute(EntityScheme.EntityName.ToString(), _getByIdOperationName, parameters);

            return getEntityRoute;
        }

        return "";
    }
}