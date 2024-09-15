using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class ListQueryGenerator : BaseGenerator<ListQueryGeneratorConfiguration>
{
    private readonly string _dtoName;
    private readonly string _handlerName;
    private readonly string _listItemDtoName;
    private readonly string _queryName;
    private readonly string _endpointClassName;

    public ListQueryGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        ListQueryGeneratorConfiguration configuration) : base(context, symbol, configuration)
    {
        _queryName = Configuration.QueryNameConfiguration.GetName(EntityName);
        _dtoName = Configuration.DtoNameConfiguration.GetName(EntityName);
        _listItemDtoName = Configuration.ListItemDtoNameConfiguration.GetName(EntityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(EntityName);
        _endpointClassName = $"Get{EntityName}ListEndpoint";
    }

    public void RunGenerator()
    {
        GenerateQuery(Configuration.QueryTemplatePath);
        GenerateListItemDto(Configuration.DtoListItemTemplatePath);
        GenerateDto(Configuration.DtoTemplatePath);
        GenerateHandler(Configuration.HandlerTemplatePath);
        GenerateEndpoint($"{Configuration.FullConfiguration.TemplatesBasePath}.GetList.GetListEndpoint.txt");
    }

    private void GenerateQuery(string templatePath)
    {
        var model = new
        {
            EntityNamespace = UsingEntityNamespace,
            QueryName = _queryName,
            PutIntoNamespace = PutIntoNamespace
        };
        WriteFile(templatePath, model, _queryName);
    }

    private void GenerateListItemDto(string templatePath)
    {
        var properties = PropertiesExtractor.GetAllPropertiesOfEntity(Symbol);
        var model = new
        {
            ListItemDtoName = _listItemDtoName,
            Properties = properties
        };

        WriteFile(templatePath, model, _listItemDtoName);
    }

    private void GenerateDto(string templatePath)
    {
        var model = new
        {
            DtoName = _dtoName,
            ListItemDtoName = _listItemDtoName
        };

        WriteFile(templatePath, model, _dtoName);
    }

    private void GenerateHandler(string templatePath)
    {
        var model = new
        {
            QueryName = _queryName,
            HandlerName = _handlerName,
            DtoName = _dtoName,
            DtoListItemName = _listItemDtoName
        };

        WriteFile(templatePath, model, _handlerName);
    }
    
    private void GenerateEndpoint(string templatePath)
    {
        var model = new
        {
            QueryNamespace = PutIntoNamespace,
            PutIntoNamespace = $"Mars.Api.Endpoints.{EntityName}Endpoints",
            EndpointClassName = _endpointClassName,
            QueryName = _queryName,
            DtoName = _dtoName
        };

        WriteFile(templatePath, model, _endpointClassName);
    }
}