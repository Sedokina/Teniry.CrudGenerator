using System.Linq;
using System.Threading;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis.CSharp;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;

internal class GetByIdQueryCrudGenerator
    : BaseOperationCrudGenerator<CqrsOperationWithReturnValueGeneratorConfiguration>
{
    private readonly string _dtoName;
    private readonly string _handlerName;
    private readonly string _queryName;
    private readonly string _endpointClassName;

    public GetByIdQueryCrudGenerator(
        CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration> scheme) : base(scheme)
    {
        _queryName = Scheme.Configuration.Operation.Name;
        _handlerName = Scheme.Configuration.Handler.Name;
        _dtoName = Scheme.Configuration.Dto.Name;
        _endpointClassName = Scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator()
    {
        GenerateQuery();
        GenerateHandler();
        GenerateDto();
        if (Scheme.Configuration.Endpoint.Generate)
        {
            GenerateEndpoint();
        }
    }

    private void GenerateQuery()
    {
        var query = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _queryName)
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .WithUsings(["ITech.Cqrs.Domain.Exceptions"])
            .WithXmlDoc($"Get {EntityScheme.EntityTitle} by id",
                $"Returns full entity data of type <see cref=\"{_dtoName}\" />",
                [
                    new ResultException(
                        "EfEntityNotFoundException",
                        $"When {Scheme.EntityScheme.EntityTitle} entity does not exist"
                    )
                ]);

        var constructorParameters = EntityScheme.PrimaryKeys
            .Select(x => new ParameterOfMethodBuilder(x.TypeName, x.PropertyNameAsMethodParameterName)).ToList();
        var constructor = new ConstructorBuilder([SyntaxKind.PublicKeyword], _queryName)
            .WithParameters(constructorParameters);
        var constructorBody = new MethodBodyBuilder();
        foreach (var primaryKey in EntityScheme.PrimaryKeys)
        {
            query.WithProperty(primaryKey.TypeName, primaryKey.PropertyName);
            constructorBody.AssignVariable(primaryKey.PropertyName, primaryKey.PropertyNameAsMethodParameterName);
        }

        constructor.WithBody(constructorBody.Build());
        query.WithConstructor(constructor.Build());

        WriteFile(_queryName, query.BuildAsString());
    }

    private void GenerateDto()
    {
        var dtoClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _dtoName)
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation);

        foreach (var property in EntityScheme.Properties)
        {
            dtoClass.WithProperty(property.TypeName, property.PropertyName, property.DefaultValue);
        }

        WriteFile(_dtoName, dtoClass.BuildAsString());
    }

    private void GenerateHandler()
    {
        var handlerClass = new ClassBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.PartialKeyword
            ], _handlerName)
            .WithUsings([
                "ITech.Cqrs.Cqrs.Queries",
                "ITech.Cqrs.Domain.Exceptions",
                Scheme.DbContextScheme.DbContextNamespace,
                EntityScheme.EntityNamespace,
                "Mapster"
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation)
            .Implements("IQueryHandler", _queryName, _dtoName)
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
                new ParameterOfMethodBuilder(_queryName, "query"),
                new ParameterOfMethodBuilder(nameof(CancellationToken), "cancellation")
            ])
            .WithXmlInheritdoc();

        var findParameters = EntityScheme.PrimaryKeys.GetAsMethodCallParameters("query");
        var methodBodyBuilder = new MethodBodyBuilder()
            .InitArrayVariable("object", "entityIds", findParameters)
            .InitVariableFromGenericAsyncMethodCall("entity", "_db", "FindAsync",
                [EntityScheme.EntityName.ToString()],
                ["entityIds", "cancellation"])
            .ThrowIfEntityNotFound("entity", EntityScheme.EntityName.ToString())
            .InitVariableFromGenericMethodCall("result", "entity", "Adapt", [_dtoName], [])
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
                "ITech.Cqrs.Cqrs.Queries",
                Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation
            ])
            .WithNamespace(Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature);

        var methodBuilder = new MethodBuilder([
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.AsyncKeyword
            ], "Task<IResult>", Scheme.Configuration.Endpoint.FunctionName)
            .WithParameters(EntityScheme.PrimaryKeys
                .Select(x => new ParameterOfMethodBuilder(x.TypeName, x.PropertyNameAsMethodParameterName))
                .Append(new ParameterOfMethodBuilder("IQueryDispatcher", "queryDispatcher"))
                .Append(new ParameterOfMethodBuilder("CancellationToken", "cancellation"))
                .ToList())
            .WithProducesResponseTypeAttribute(_dtoName)
            .WithXmlDoc($"Get {Scheme.EntityScheme.EntityTitle} by id",
                200,
                $"Returns full {Scheme.EntityScheme.EntityTitle} data");

        var methodBodyBuilder = new MethodBodyBuilder()
            .InitVariableFromConstructorCall("query", _queryName, EntityScheme.PrimaryKeys)
            .InitVariableFromGenericAsyncMethodCall("result", "queryDispatcher", "DispatchAsync",
                [_queryName, _dtoName],
                ["query", "cancellation"])
            .ReturnTypedResultOk("result");

        methodBuilder.WithBody(methodBodyBuilder.Build());
        endpointClass.WithMethod(methodBuilder.Build());

        WriteFile(_endpointClassName, endpointClass.BuildAsString());

        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Get",
            Scheme.Configuration.Endpoint.Route,
            $"{_endpointClassName}.{Scheme.Configuration.Endpoint.FunctionName}");
    }
}