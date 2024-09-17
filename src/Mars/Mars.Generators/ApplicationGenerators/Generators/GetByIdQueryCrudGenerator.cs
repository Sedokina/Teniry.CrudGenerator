using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class GetByIdQueryCrudGenerator : BaseCrudGenerator<BaseQueryGeneratorConfiguration>
{
    private readonly string _dtoName;
    private readonly string _handlerName;
    private readonly string _queryName;
    private readonly string _endpointClassName;

    public GetByIdQueryCrudGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        BaseQueryGeneratorConfiguration configuration,
        EntityConfiguration entityConfiguration) : base(context, symbol, configuration, entityConfiguration)
    {
        _queryName = Configuration.QueryNameConfiguration.GetName(EntityName);
        _dtoName = Configuration.DtoNameConfiguration.GetName(EntityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(EntityName);
        _endpointClassName = Configuration.EndpointNameConfiguration.GetName(EntityName);
    }

    public override void RunGenerator()
    {
        GenerateQuery(Configuration.QueryTemplatePath);
        GenerateDto(Configuration.DtoTemplatePath);
        GenerateHandler(Configuration.HandlerTemplatePath);
        GenerateEndpoint(Configuration.EndpointTemplatePath);
    }

    private void GenerateQuery(string templatePath)
    {
        var primaryKeys = PropertiesExtractor.GetPrimaryKeysOfEntity(Symbol);
        var properties = primaryKeys.ToClassPropertiesString();
        var constructorParameters = primaryKeys.ToMethodParametersString();
        var constructorBody = primaryKeys.ToConstructorBodyString();
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
        var properties = PropertiesExtractor.GetAllPropertiesOfEntity(Symbol);
        var model = new
        {
            DtoName = _dtoName,
            Properties = properties
        };

        WriteFile(templatePath, model, _dtoName);
    }

    private void GenerateHandler(string templatePath)
    {
        var properties = PropertiesExtractor.GetPrimaryKeysOfEntity(Symbol).ToPropertiesNamesList("query");
        var model = new
        {
            QueryName = _queryName,
            HandlerName = _handlerName,
            DtoName = _dtoName,
            FindProperties = string.Join(", ", properties)
        };

        WriteFile(templatePath, model, _handlerName);
    }

    private void GenerateEndpoint(string templatePath)
    {
        var primaryKeys = PropertiesExtractor.GetPrimaryKeysOfEntity(Symbol);
        var routeParams = primaryKeys.ToMethodParametersString();
        var constructorParams = primaryKeys.ToPropertiesNamesList();
        var model = new
        {
            EndpointClassName = _endpointClassName,
            QueryName = _queryName,
            DtoName = _dtoName,
            RouteParams = routeParams,
            QueryConstructorParameters = string.Join(", ", constructorParams)
        };

        WriteFile(templatePath, model, _endpointClassName);
        EndpointMap = new EndpointMap(EntityName,
            EndpointNamespace,
            "Get",
            Configuration.EndpointRouteConfiguration.GetRoute(EntityName, constructorParams),
            $"{_endpointClassName}.{Configuration.EndpointRouteConfiguration.FunctionName}");
    }
}