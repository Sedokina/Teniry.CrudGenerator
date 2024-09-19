using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.DbContextCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;
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
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme) : base(context, symbol, configuration, entityScheme, dbContextScheme)
    {
        _queryName = Configuration.QueryNameConfiguration.GetName(EntityScheme.EntityName);
        _dtoName = Configuration.DtoNameConfiguration.GetName(EntityScheme.EntityName);
        _listItemDtoName = Configuration.ListItemDtoNameConfiguration.GetName(EntityScheme.EntityName);
        _filterName = Configuration.FilterNameConfiguration.GetName(EntityScheme.EntityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(EntityScheme.EntityName);
        _endpointClassName = Configuration.EndpointNameConfiguration.GetName(EntityScheme.EntityName);
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
        var properties = EntityScheme.Properties.FormatAsFilterProperties();
        var sortKeys = EntityScheme.SortableProperties.FormatAsSortKeys();

        var model = new
        {
            QueryName = _queryName,
            DtoName = _dtoName,
            PutIntoNamespace = BusinessLogicNamespace,
            Properties = properties,
            SortKeys = sortKeys
        };
        WriteFile(templatePath, model, _queryName);
    }

    private void GenerateListItemDto(string templatePath)
    {
        var properties = EntityScheme.Properties.FormatAsProperties();
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
        var properties = EntityScheme.Properties.FormatAsFilterProperties();
        var filter = EntityScheme.Properties.FormatAsFilterBody();
        var sorts = EntityScheme.SortableProperties.FormatAsSortCalls();

        var model = new
        {
            FilterName = _filterName,
            Properties = properties,
            Filter = filter,
            Sorts = sorts
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
            DtoListItemName = _listItemDtoName,
            FilterName = _filterName
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
        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            EndpointNamespace,
            "Get",
            Configuration.EndpointRouteConfiguration.GetRoute(EntityScheme.EntityName.ToString()),
            $"{_endpointClassName}.{Configuration.EndpointRouteConfiguration.FunctionName}");
    }
}