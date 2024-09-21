using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

internal class GetByIdQueryCrudGenerator : BaseCrudGenerator<CqrsWithReturnValueConfiguration>
{
    private readonly string _dtoName;
    private readonly string _handlerName;
    private readonly string _queryName;
    private readonly string _endpointClassName;

    public GetByIdQueryCrudGenerator(
        GeneratorExecutionContext context,
        CrudGeneratorScheme<CqrsWithReturnValueConfiguration> scheme) : base(context, scheme)
    {
        _queryName = Scheme.Configuration.Operation.GetName(EntityScheme.EntityName);
        _handlerName = Scheme.Configuration.Handler.GetName(EntityScheme.EntityName);
        _dtoName = Scheme.Configuration.Dto.GetName(EntityScheme.EntityName);
        _endpointClassName = Scheme.Configuration.Endpoint.GetName(EntityScheme.EntityName);
    }

    public override void RunGenerator()
    {
        GenerateQuery(Scheme.Configuration.Operation.TemplatePath);
        GenerateHandler(Scheme.Configuration.Handler.TemplatePath);
        GenerateDto(Scheme.Configuration.Dto.TemplatePath);
        GenerateEndpoint(Scheme.Configuration.Endpoint.TemplatePath);
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

    private void GenerateEndpoint(string templatePath)
    {
        var routeParams = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters();
        var model = new
        {
            EndpointClassName = _endpointClassName,
            QueryName = _queryName,
            DtoName = _dtoName,
            RouteParams = routeParams,
            QueryConstructorParameters = constructorParameters
        };

        WriteFile(templatePath, model, _endpointClassName);

        var constructorParametersForRoute = EntityScheme.PrimaryKeys.GetAsMethodCallParameters();
        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            EndpointNamespace,
            "Get",
            Scheme.Configuration.Endpoint.RouteConfiguration
                .GetRoute(EntityScheme.EntityName.ToString(), constructorParametersForRoute),
            $"{_endpointClassName}.{Scheme.Configuration.Endpoint.RouteConfiguration.FunctionName}");
    }
}