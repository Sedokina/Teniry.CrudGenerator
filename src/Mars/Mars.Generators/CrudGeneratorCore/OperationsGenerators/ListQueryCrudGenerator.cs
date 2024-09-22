using Mars.Generators.ApplicationGenerators.Configurations.Operations;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

internal class ListQueryCrudGenerator : BaseOperationCrudGenerator<CqrsListOperationGeneratorConfiguration>
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
        _queryName = Scheme.Configuration.Operation.Name;
        _listItemDtoName = Scheme.Configuration.DtoListItem.Name;
        _dtoName = Scheme.Configuration.Dto.Name;
        _filterName = Scheme.Configuration.Filter.Name;
        _handlerName = Scheme.Configuration.Handler.Name;
        _endpointClassName = Scheme.Configuration.Endpoint.Name;
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
            PutIntoNamespace = Scheme.Configuration.OperationsSharedConfiguration.BusinessLogicNamespaceForOperation,
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
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Get",
            Scheme.Configuration.Endpoint.Route,
            $"{_endpointClassName}.{Scheme.Configuration.Endpoint.FunctionName}");
    }
}