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
        BaseQueryGeneratorConfiguration configuration) : base(context, symbol, configuration)
    {
        _queryName = Configuration.QueryNameConfiguration.GetName(EntityName);
        _dtoName = Configuration.DtoNameConfiguration.GetName(EntityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(EntityName);
        _endpointClassName = $"Get{EntityName}Endpoint";
    }

    public override void RunGenerator()
    {
        GenerateQuery(Configuration.QueryTemplatePath);
        GenerateDto(Configuration.DtoTemplatePath);
        GenerateHandler(Configuration.HandlerTemplatePath);
        GenerateEndpoint($"{Configuration.FullConfiguration.TemplatesBasePath}.GetById.GetByIdEndpoint.txt");
    }

    private void GenerateQuery(string templatePath)
    {
        var properties = PropertiesExtractor.GetPrimaryKeysOfEntityAsProperties(Symbol);
        var model = new
        {
            QueryName = _queryName,
            Properties = properties
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
        var properties = PropertiesExtractor.GetPrimaryKeyNamesOfEntity(Symbol, "query");
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
        var endpointNamespace = $"Mars.Api.Endpoints.{EntityName}Endpoints";
        var model = new
        {
            QueryNamespace = PutIntoNamespace,
            PutIntoNamespace = endpointNamespace,
            EndpointClassName = _endpointClassName,
            QueryName = _queryName,
            DtoName = _dtoName
        };

        WriteFile(templatePath, model, _endpointClassName);
        EndpointMapCall = (endpointNamespace, $".MapGet(\"/{EntityName.ToLower()}\", {_endpointClassName}.GetAsync)");
    }
}