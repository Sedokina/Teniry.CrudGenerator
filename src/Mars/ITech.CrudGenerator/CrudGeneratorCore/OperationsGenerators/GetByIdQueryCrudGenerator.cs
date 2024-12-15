using System.Linq;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Formatters;
using Microsoft.CodeAnalysis.CSharp;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;

internal class
    GetByIdQueryCrudGenerator : BaseOperationCrudGenerator<CqrsOperationWithReturnValueGeneratorConfiguration>
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
        GenerateQuery(Scheme.Configuration.Operation.TemplatePath);
        GenerateHandler(Scheme.Configuration.Handler.TemplatePath);
        GenerateDto(Scheme.Configuration.Dto.TemplatePath);
        if (Scheme.Configuration.Endpoint.Generate)
        {
            GenerateEndpoint();
        }
    }

    private void GenerateQuery(string templatePath)
    {
        var properties = EntityScheme.PrimaryKeys.FormatAsProperties();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorBody = EntityScheme.PrimaryKeys.FormatAsConstructorBody();
        var model = new
        {
            QueryName = _queryName,
            DtoName = _dtoName,
            Properties = properties,
            ConstructorParameters = constructorParameters,
            ConstructorBody = constructorBody
        };

        WriteFile(templatePath, model, _queryName);
    }

    private void GenerateDto(string templatePath)
    {
        var properties = EntityScheme.Properties.FormatAsProperties();
        var model = new
        {
            DtoName = _dtoName,
            Properties = properties
        };

        WriteFile(templatePath, model, _dtoName);
    }

    private void GenerateHandler(string templatePath)
    {
        var findParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters("query");
        var model = new
        {
            QueryName = _queryName,
            HandlerName = _handlerName,
            DtoName = _dtoName,
            FindParameters = findParameters
        };

        WriteFile(templatePath, model, _handlerName);
    }

    private void GenerateEndpoint()
    {
        var endpointClass = new ClassBuilder(_endpointClassName)
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
            .InitVariableFromGenericAsyncMethodCall("result", "queryDispatcher", "DispatchAsync", [_queryName, _dtoName],
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