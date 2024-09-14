using System;
using System.Text;
using Mars.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Mars.Generators;

[Generator]
public class GetListQueryGenerator : ISourceGenerator
{
    private const string QueryResourcePath = "Mars.Generators.Templates.GetListQuery.txt";
    private const string DtoListItemResourcePath = "Mars.Generators.Templates.GetListItemDto.txt";
    private const string DtoResourcePath = "Mars.Generators.Templates.GetListDto.txt";
    private const string HandlerResourcePath = "Mars.Generators.Templates.GetListHandler.txt";

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
            GenerateListItemDto(context, symbol);
            GenerateListDto(context, symbol);
            GenerateHandler(context, symbol);
        }
    }


    private void GenerateQuery(GeneratorExecutionContext context, ISymbol symbol)
    {
        var template = Template
            .Parse(EmbeddedResourceExtensions.GetEmbeddedResource(QueryResourcePath, GetType().Assembly));

        var sourceCode = template.Render(new
        {
            ClassName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name,
        });

        context.AddSource(
            $"Get{symbol.Name}ListQuery.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateListItemDto(GeneratorExecutionContext context, ISymbol symbol)
    {
        var template = Template
            .Parse(EmbeddedResourceExtensions.GetEmbeddedResource(DtoListItemResourcePath, GetType().Assembly));

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
            $"{symbol.Name}ListItemDto.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateListDto(GeneratorExecutionContext context, ISymbol symbol)
    {
        var template = Template
            .Parse(EmbeddedResourceExtensions.GetEmbeddedResource(DtoResourcePath, GetType().Assembly));

        var sourceCode = template.Render(new
        {
            ClassName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name,
            ItemsType = $"{symbol.Name}ListItemDto"
        });

        context.AddSource(
            $"{symbol.Name}ListDto.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateHandler(GeneratorExecutionContext context, ISymbol symbol)
    {
        var template = Template
            .Parse(EmbeddedResourceExtensions.GetEmbeddedResource(HandlerResourcePath, GetType().Assembly));

        var sourceCode = template.Render(new
        {
            EntityName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name,
            QueryName = $"Get{symbol.Name}ListQuery",
            DtoName = $"{symbol.Name}ListDto",
            DtoListItemName = $"{symbol.Name}ListItemDto",
        });

        context.AddSource(
            $"Get{symbol.Name}ListHandler.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }
}