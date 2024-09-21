using Mars.Generators.ApplicationGenerators.Configurations;
using Mars.Generators.ApplicationGenerators.Configurations.Operations;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

internal class ListQueryCrudGenerator : BaseCrudGenerator<CqrsListOperationGeneratorConfiguration>
{
    private readonly string _dtoName;
    private readonly string _handlerName;
    private readonly string _listItemDtoName;
    private readonly string _queryName;
    private readonly string _endpointClassName;
    private readonly string _filterName;

    public ListQueryCrudGenerator(
        GeneratorExecutionContext context,
        CrudGeneratorScheme<CqrsListOperationGeneratorConfiguration> scheme) : base(context, scheme)
    {
        _queryName = Scheme.Configuration.Operation.GetName(EntityScheme.EntityName);
        _listItemDtoName = Scheme.Configuration.DtoListItem.GetName(EntityScheme.EntityName);
        _dtoName = Scheme.Configuration.Dto.GetName(EntityScheme.EntityName);
        _filterName = Scheme.Configuration.Filter.GetName(EntityScheme.EntityName);
        _handlerName = Scheme.Configuration.Handler.GetName(EntityScheme.EntityName);
        _endpointClassName = Scheme.Configuration.Endpoint.GetName(EntityScheme.EntityName);
    }

    public override void RunGenerator()
    {
        GenerateQuery(Scheme.Configuration.Operation.TemplatePath);
        GenerateListItemDto(Scheme.Configuration.DtoListItem.TemplatePath);
        GenerateDto(Scheme.Configuration.Dto.TemplatePath);
        GenerateFilter(Scheme.Configuration.Filter.TemplatePath);
        GenerateHandler(Scheme.Configuration.Handler.TemplatePath);
        GenerateEndpoint(Scheme.Configuration.Endpoint.TemplatePath);
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
        var defaultSort = FormatDefaultSort(EntityScheme.DefaultSort);

        var model = new
        {
            FilterName = _filterName,
            Properties = properties,
            Filter = filter,
            Sorts = sorts,
            DefaultSort = defaultSort
        };
        WriteFile(templatePath, model, _filterName);
    }

    private static string FormatDefaultSort(EntityDefaultSort? defaultSort)
    {
        if (defaultSort != null)
        {
            return defaultSort.Direction.Equals("asc")
                ? $"query.OrderBy(x => x.{defaultSort.PropertyName});"
                : $"query.OrderByDescending(x => x.{defaultSort.PropertyName});";
        }

        return "base.DefaultSort(query);";
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
            Scheme.Configuration.Endpoint.RouteConfiguration.GetRoute(EntityScheme.EntityName.ToString()),
            $"{_endpointClassName}.{Scheme.Configuration.Endpoint.RouteConfiguration.FunctionName}");
    }
}