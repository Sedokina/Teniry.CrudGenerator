using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class ListQueryGenerator : BaseGenerator<ListQueryGeneratorConfiguration>
{
    private readonly string _dtoName;
    private readonly string _handlerName;
    private readonly string _listItemDtoName;
    private readonly string _queryName;

    public ListQueryGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        ListQueryGeneratorConfiguration configuration) : base(context, symbol, configuration)
    {
        _queryName = Configuration.QueryNameConfiguration.GetName(_entityName);
        _dtoName = Configuration.DtoNameConfiguration.GetName(_entityName);
        _listItemDtoName = Configuration.ListItemDtoNameConfiguration.GetName(_entityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(_entityName);
    }

    public void RunGenerator()
    {
        GenerateQuery(Configuration.QueryTemplatePath);
        GenerateListItemDto(Configuration.DtoListItemTemplatePath);
        GenerateDto(Configuration.DtoTemplatePath);
        GenerateHandler(Configuration.HandlerTemplatePath);
    }

    private void GenerateQuery(string templatePath)
    {
        var model = new
        {
            EntityNamespace = _usingEntityNamespace,
            QueryName = _queryName,
            PutIntoNamespace = _putIntoNamespace
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
            PutIntoNamespace = _putIntoNamespace,
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
}