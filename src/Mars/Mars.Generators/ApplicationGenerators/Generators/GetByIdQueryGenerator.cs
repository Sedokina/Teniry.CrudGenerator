using System;
using System.Collections.Generic;
using System.Text;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Mars.Generators.ApplicationGenerators.Generators;

[Generator]
public class GetByIdQueryGenerator : CrudGenerator, ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver<GenerateCreateCommandAttribute>());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not AttributeSyntaxReceiver<GenerateCreateCommandAttribute> syntaxReceiver)
        {
            return;
        }

        foreach (var classSyntax in syntaxReceiver.Classes)
        {
            // Converting the class to semantic model to access much more meaningful data.
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            // Parse to declared symbol, so you can access each part of code separately, such as interfaces, methods, members, contructor parameters etc.
            var symbol = model.GetDeclaredSymbol(classSyntax) ?? throw new ArgumentException("symbol");

            GenerateQuery(context, symbol);
            GenerateDto(context, symbol);
            GenerateHandler(context, symbol);
        }
    }

    private void GenerateQuery(GeneratorExecutionContext context, ISymbol symbol)
    {
        var template = ReadTemplate(Configuration.GetByIdQueryGenerator.QueryTemplatePath);

        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to query property if it is not id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (!propertyNameLower.Equals("id") && !propertyNameLower.Equals($"{symbol.Name}id"))
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
            ClassName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name,
            Properties = result
        });

        context.AddSource(
            $"Get{symbol.Name}Query.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateDto(GeneratorExecutionContext context, ISymbol symbol)
    {
        var template = ReadTemplate(Configuration.GetByIdQueryGenerator.DtoTemplatePath);
        
        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
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
            ClassName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name,
            Properties = result,
        });

        context.AddSource(
            $"{symbol.Name}Dto.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateHandler(GeneratorExecutionContext context, ISymbol symbol)
    {
        var template = ReadTemplate(Configuration.GetByIdQueryGenerator.HandlerTemplatePath);

        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = new List<string>();
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to query property if it is not id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (!propertyNameLower.Equals("id") && !propertyNameLower.Equals($"{symbol.Name}id"))
            {
                continue;
            }

            result.Add($"query.{propertySymbol.Name}");
        }

        var sourceCode = template.Render(new
        {
            EntityName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name,
            QueryName = $"Get{symbol.Name}Query",
            DtoName = $"{symbol.Name}Dto",
            FindProperties = string.Join(", ", result)
        });

        context.AddSource(
            $"Get{symbol.Name}Handler.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }
}