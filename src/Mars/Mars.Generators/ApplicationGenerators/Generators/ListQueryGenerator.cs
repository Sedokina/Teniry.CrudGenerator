using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class ListQueryGenerator : BaseGenerator
{
    private readonly string _entityName;
    private readonly string _usingEntityNamespace;
    private readonly string _putIntoNamespace;
    private readonly string _queryName;
    private readonly string _dtoName;
    private readonly string _listItemDtoName;
    private readonly string _handlerName;

    public ListQueryGenerator(GeneratorExecutionContext context, ISymbol symbol) : base(context, symbol)
    {
        _entityName = Symbol.Name;
        _usingEntityNamespace = Symbol.ContainingNamespace.ToString();
        _putIntoNamespace = Symbol.ContainingAssembly.Name;
        _queryName = Configuration.GetListQueryGenerator.GetQueryName(_entityName);
        _dtoName = Configuration.GetListQueryGenerator.GetDtoName(_entityName);
        _listItemDtoName = Configuration.GetListQueryGenerator.GetListItemDtoName(_entityName);
        _handlerName = Configuration.GetListQueryGenerator.GetHandlerName(_entityName);
    }

    public void RunGenerator()
    {
        GenerateQuery(Configuration.GetListQueryGenerator.QueryTemplatePath);
        GenerateListItemDto(Configuration.GetListQueryGenerator.DtoListItemTemplatePath);
        GenerateDto(Configuration.GetListQueryGenerator.DtoTemplatePath);
        GenerateHandler(Configuration.GetListQueryGenerator.HandlerTemplatePath);
    }

    private void GenerateQuery(string templatePath)
    {
        var template = ReadTemplate(templatePath);

        var sourceCode = template.Render(new
        {
            EntityNamespace = _usingEntityNamespace,
            QueryName = _queryName,
            PutIntoNamespace = _putIntoNamespace,
        });

        WriteFile(_queryName, sourceCode);
    }

    private void GenerateListItemDto(string templatePath)
    {
        var template = ReadTemplate(templatePath);

        var propertiesOfClass = ((INamedTypeSymbol)Symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to query if not primitive type
            if (!propertySymbol.Type.IsSimple())
            {
                continue;
            }

            // For DateTimeOffset and other date variations remove system from the property type declaration
            var propertyTypeName = propertySymbol.Type.ToString().ToLower().StartsWith("system.")
                ? propertySymbol.Type.MetadataName
                : propertySymbol.Type.ToString();

            result += $"public {propertyTypeName} {propertySymbol.Name} {{ get; set; }}\n\t";
        }

        result = result.TrimEnd();

        var sourceCode = template.Render(new
        {
            PutIntoNamespace = _putIntoNamespace,
            ListItemDtoName = _listItemDtoName,
            Properties = result,
        });

        WriteFile(_listItemDtoName, sourceCode);
    }

    private void GenerateDto(string templatePath)
    {
        var template = ReadTemplate(templatePath);

        var sourceCode = template.Render(new
        {
            PutIntoNamespace = _putIntoNamespace,
            DtoName = _dtoName,
            ListItemDtoName = _listItemDtoName
        });

        WriteFile(_dtoName, sourceCode);
    }

    private void GenerateHandler(string templatePath)
    {
        var template = ReadTemplate(templatePath);

        var sourceCode = template.Render(new
        {
            EntityName = _entityName,
            EntityNamespace = _usingEntityNamespace,
            PutIntoNamespace = _putIntoNamespace,
            QueryName = _queryName,
            HandlerName = _handlerName,
            DtoName = _dtoName,
            DtoListItemName = _listItemDtoName,
        });

        WriteFile(_handlerName, sourceCode);
    }
}