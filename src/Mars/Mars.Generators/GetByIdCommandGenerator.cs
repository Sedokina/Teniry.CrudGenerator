using System;
using System.Collections.Generic;
using System.Text;
using Mars.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Mars.Generators;

[Generator]
public class GetByIdCommandGenerator : ISourceGenerator
{
    private const string CommandResourcePath = "Mars.Generators.Templates.GetByIdCommand.txt";
    private const string DtoResourcePath = "Mars.Generators.Templates.GetByIdDto.txt";
    private const string HandlerResourcePath = "Mars.Generators.Templates.GetByIdHandler.txt";

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

            GenerateCommand(context, symbol);
            GenerateDto(context, symbol);
            GenerateHandler(context, symbol);
        }
    }

    private void GenerateCommand(GeneratorExecutionContext context, ISymbol symbol)
    {
        var template = Template
            .Parse(EmbeddedResourceExtensions.GetEmbeddedResource(CommandResourcePath, GetType().Assembly));

        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to command property if it is not id of the entity
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
            $"Get{symbol.Name}Command.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }
    
    private void GenerateDto(GeneratorExecutionContext context, ISymbol symbol)
    {
        var template = Template
            .Parse(EmbeddedResourceExtensions.GetEmbeddedResource(DtoResourcePath, GetType().Assembly));

        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to command if not primitive type
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
        var template = Template
            .Parse(EmbeddedResourceExtensions.GetEmbeddedResource(HandlerResourcePath, GetType().Assembly));

        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = new List<string>();
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to command property if it is not id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (!propertyNameLower.Equals("id") && !propertyNameLower.Equals($"{symbol.Name}id"))
            {
                continue;
            }

            result.Add($"command.{propertySymbol.Name}");
        }

        var sourceCode = template.Render(new
        {
            EntityName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name,
            CommandName = $"Get{symbol.Name}Command",
            DtoName = $"{symbol.Name}Dto",
            FindProperties = string.Join(", ", result)
        });

        context.AddSource(
            $"Get{symbol.Name}Handler.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }
}