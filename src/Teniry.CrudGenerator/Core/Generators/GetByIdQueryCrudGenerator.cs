using System.Linq;
using System.Threading;
using Teniry.CrudGenerator.Core.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Teniry.CrudGenerator.Core.Configurations.Crud;
using Teniry.CrudGenerator.Core.Generators.Core;
using Teniry.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders;
using Teniry.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders.Models;
using static Teniry.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders.SimpleSyntaxFactory;

namespace Teniry.CrudGenerator.Core.Generators;

internal class GetByIdQueryCrudGenerator
    : BaseOperationCrudGenerator<CqrsOperationWithReturnValueGeneratorConfiguration> {
    private readonly string _dtoName;
    private readonly string _endpointClassName;
    private readonly string _handlerName;
    private readonly string _queryName;

    public GetByIdQueryCrudGenerator(
        CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration> scheme
    ) : base(scheme) {
        _queryName = Scheme.Configuration.Operation;
        _handlerName = Scheme.Configuration.Handler;
        _dtoName = Scheme.Configuration.Dto;
        _endpointClassName = Scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator() {
        GenerateQuery();
        GenerateHandler();
        GenerateDto();
        if (Scheme.Configuration.Endpoint.Generate) {
            GenerateEndpoint();
        }
    }

    private void GenerateQuery() {
        var query = new ClassBuilder(
                [
                    SyntaxKind.PublicKeyword,
                    SyntaxKind.PartialKeyword
                ],
                _queryName
            )
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .WithUsings(["Teniry.Cqrs.Extended.Exceptions"])
            .WithXmlDoc(
                $"Get {EntityScheme.EntityTitle} by id",
                $"Returns full entity data of type <see cref=\"{_dtoName}\" />",
                [
                    new(
                        "EntityNotFoundException",
                        $"When {Scheme.EntityScheme.EntityTitle} entity does not exist"
                    )
                ]
            );

        var constructorParameters = EntityScheme.PrimaryKeys
            .Select(x => new ParameterOfMethodBuilder(x.TypeName, x.PropertyNameAsMethodParameterName)).ToList();
        var constructor = new ConstructorBuilder(_queryName)
            .WithParameters(constructorParameters);
        var constructorBody = new BlockBuilder();
        foreach (var primaryKey in EntityScheme.PrimaryKeys) {
            query.WithProperty(primaryKey.TypeName, primaryKey.PropertyName);
            constructorBody.AssignVariable(primaryKey.PropertyName, primaryKey.PropertyNameAsMethodParameterName);
        }

        constructor.WithBody(constructorBody);
        query.WithConstructor(constructor.Build());

        WriteFile(_queryName, query.BuildAsString());
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

        foreach (var property in EntityScheme.Properties) {
            dtoClass.WithProperty(property.TypeName, property.PropertyName)
                .WithDefaultValue(property.DefaultValue);
        }

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
                    "Teniry.Cqrs.Queries",
                    "Teniry.Cqrs.Extended.Exceptions",
                    Scheme.DbContextScheme.DbContextNamespace,
                    EntityScheme.EntityNamespace,
                    "Mapster"
                ]
            )
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .Implements("IQueryHandler", _queryName, _dtoName)
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
                    new ParameterOfMethodBuilder(_queryName, "query"),
                    new ParameterOfMethodBuilder(nameof(CancellationToken), "cancellation")
                ]
            )
            .WithXmlInheritdoc();

        var findParameters = EntityScheme.PrimaryKeys.GetAsMethodCallParameters("query");
        var methodBodyBuilder = new BlockBuilder()
            .InitVariable(
                "entity",
                CallGenericAsyncMethod(
                    "_db",
                    "FindAsync",
                    [EntityScheme.EntityName.ToString()],
                    [NewArray("object", findParameters), Variable("cancellation")]
                )
            )
            .IfNull("entity", builder => builder.ThrowEntityNotFoundException(EntityScheme.EntityName.ToString()))
            .Return(CallGenericMethod("entity", "Adapt", [_dtoName], []));

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
                    "Teniry.Cqrs.Queries",
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
                EntityScheme.PrimaryKeys
                    .Select(x => new ParameterOfMethodBuilder(x.TypeName, x.PropertyNameAsMethodParameterName))
                    .Append(new("IQueryDispatcher", "queryDispatcher"))
                    .Append(new("CancellationToken", "cancellation"))
                    .ToList()
            )
            .WithAttribute(new(_dtoName))
            .WithXmlDoc(
                $"Get {Scheme.EntityScheme.EntityTitle} by id",
                200,
                $"Returns full {Scheme.EntityScheme.EntityTitle} data"
            );

        var methodBodyBuilder = new BlockBuilder()
            .InitVariable(
                "query",
                CallConstructor(
                    _queryName,
                    EntityScheme.PrimaryKeys
                        .Select(x => Variable(x.PropertyNameAsMethodParameterName))
                        .ToList<ExpressionSyntax>()
                )
            )
            .InitVariable(
                "result",
                CallGenericAsyncMethod(
                    "queryDispatcher",
                    "DispatchAsync",
                    [_queryName, _dtoName],
                    [Variable("query"), Variable("cancellation")]
                )
            )
            .Return(CallMethod("TypedResults", "Ok", [Variable("result")]));

        methodBuilder.WithBody(methodBodyBuilder);
        endpointClass.WithMethod(methodBuilder.Build());

        WriteFile(_endpointClassName, endpointClass.BuildAsString());

        EndpointMap = new(
            EntityScheme.EntityTitle.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Get",
            Scheme.Configuration.Endpoint.Route,
            _endpointClassName,
            Scheme.Configuration.Endpoint.FunctionName
        );
    }
}