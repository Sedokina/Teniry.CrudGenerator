using System.Collections.Generic;
using System.Text;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class GenerateGetByIdQuery : CrudGenerator
{
    private readonly GeneratorExecutionContext _context;
    private readonly ISymbol _symbol;
    private readonly string _entityName;
    private readonly string _usingEntityNamespace;
    private readonly string _putIntoNamespace;
    private readonly string _queryName;
    private readonly string _dtoName;
    private readonly string _handlerName;

    public GenerateGetByIdQuery(GeneratorExecutionContext context, ISymbol symbol)
    {
        _context = context;
        _symbol = symbol;
        _entityName = _symbol.Name;
        _usingEntityNamespace = _symbol.ContainingNamespace.ToString();
        _putIntoNamespace = _symbol.ContainingAssembly.Name;
        _queryName = Configuration.GetByIdQueryGenerator.GetQueryName(_entityName);
        _dtoName = Configuration.GetByIdQueryGenerator.GetDtoName(_entityName);
        _handlerName = Configuration.GetByIdQueryGenerator.GetHandlerName(_entityName);
    }

    public void RunGenerator()
    {
        GenerateQuery(Configuration.GetByIdQueryGenerator.QueryTemplatePath);
        GenerateDto(Configuration.GetByIdQueryGenerator.DtoTemplatePath);
        GenerateHandler(Configuration.GetByIdQueryGenerator.HandlerTemplatePath);
    }

    private void GenerateQuery(string templatePath)
    {
        var template = ReadTemplate(templatePath);

        var propertiesOfClass = ((INamedTypeSymbol)_symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to query property if it is not id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (!propertyNameLower.Equals("id") && !propertyNameLower.Equals($"{_symbol.Name}id"))
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
            EntityName = _entityName,
            EntityNamespace = _usingEntityNamespace,
            PutIntoNamespace = _putIntoNamespace,
            Properties = result
        });

        _context.AddSource($"{_queryName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateDto(string templatePath)
    {
        var template = ReadTemplate(templatePath);

        var propertiesOfClass = ((INamedTypeSymbol)_symbol).GetMembers().OfType<IPropertySymbol>();
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
            EntityName = _entityName,
            EntityNamespace = _usingEntityNamespace,
            PutIntoNamespace = _putIntoNamespace,
            Properties = result,
        });

        _context.AddSource($"{_dtoName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateHandler(string templatePath)
    {
        var template = ReadTemplate(templatePath);

        var propertiesOfClass = ((INamedTypeSymbol)_symbol).GetMembers().OfType<IPropertySymbol>();
        var result = new List<string>();
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to query property if it is not id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (!propertyNameLower.Equals("id") && !propertyNameLower.Equals($"{_symbol.Name}id"))
            {
                continue;
            }

            result.Add($"query.{propertySymbol.Name}");
        }

        var sourceCode = template.Render(new
        {
            EntityName = _entityName,
            EntityNamespace = _usingEntityNamespace,
            PutIntoNamespace = _putIntoNamespace,
            QueryName = _queryName,
            DtoName = $"{_symbol.Name}Dto",
            FindProperties = string.Join(", ", result)
        });

        _context.AddSource($"{_handlerName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}