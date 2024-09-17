using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class ListQueryCrudGenerator : BaseCrudGenerator<ListQueryGeneratorConfiguration>
{
    private readonly string _dtoName;
    private readonly string _handlerName;
    private readonly string _listItemDtoName;
    private readonly string _queryName;
    private readonly string _endpointClassName;
    private readonly string _filterName;

    public ListQueryCrudGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        ListQueryGeneratorConfiguration configuration,
        EntityConfiguration entityConfiguration) : base(context, symbol, configuration, entityConfiguration)
    {
        _queryName = Configuration.QueryNameConfiguration.GetName(EntityName);
        _dtoName = Configuration.DtoNameConfiguration.GetName(EntityName);
        _listItemDtoName = Configuration.ListItemDtoNameConfiguration.GetName(EntityName);
        _filterName = Configuration.FilterNameConfiguration.GetName(EntityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(EntityName);
        _endpointClassName = Configuration.EndpointNameConfiguration.GetName(EntityName);
    }

    public override void RunGenerator()
    {
        GenerateQuery(Configuration.QueryTemplatePath);
        GenerateListItemDto(Configuration.DtoListItemTemplatePath);
        GenerateDto(Configuration.DtoTemplatePath);
        GenerateFilter(Configuration.FilterTemplatePath);
        GenerateHandler(Configuration.HandlerTemplatePath);
        GenerateEndpoint(Configuration.EndpointTemplatePath);
    }

    private void GenerateQuery(string templatePath)
    {
        var properties = PropertiesExtractor.GetAllPropertiesOfEntityForFilter(Symbol)
            .ToClassPropertiesString();

        var model = new
        {
            EntityNamespace = UsingEntityNamespace,
            QueryName = _queryName,
            DtoName = _dtoName,
            PutIntoNamespace = BusinessLogicNamespace,
            Properties = properties
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

    private void GenerateFilter(string templatePath)
    {
        var properties = PropertiesExtractor.GetAllPropertiesOfEntityForFilter(Symbol)
            .ToClassPropertiesString();

        var model = new
        {
            EntityNamespace = UsingEntityNamespace,
            FilterName = _filterName,
            Properties = properties
        };
        WriteFile(templatePath, model, _filterName);
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
            EndpointClassName = _endpointClassName,
            QueryName = _queryName,
            DtoName = _dtoName
        };

        WriteFile(templatePath, model, _endpointClassName);
        EndpointMap = new EndpointMap(EntityName,
            EndpointNamespace,
            "Get",
            Configuration.EndpointRouteConfiguration.GetRoute(EntityName),
            $"{_endpointClassName}.{Configuration.EndpointRouteConfiguration.FunctionName}");
    }
}