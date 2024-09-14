using System;
using System.Text;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Mars.Generators.ApplicationGenerators.Generators;

[Generator]
public class CreateCommandGenerator : CrudGenerator, ISourceGenerator
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

            GenerateCommand(context, symbol);
            GenerateHandler(context, symbol);
        }
    }

    private void GenerateCommand(GeneratorExecutionContext context, ISymbol symbol)
    {
        var template = ReadTemplate(Configuration.CreateCommandCommandGenerator.CommandTemplatePath);

        var propertiesOfClass = ((INamedTypeSymbol)symbol).GetMembers().OfType<IPropertySymbol>();
        var result = "";
        foreach (var propertySymbol in propertiesOfClass)
        {
            // skip adding to command id of the entity
            var propertyNameLower = propertySymbol.Name.ToLower();
            if (propertyNameLower.Equals("id") || propertyNameLower.Equals($"{symbol.Name}id"))
            {
                continue;
            }

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
            Properties = result
        });

        context.AddSource(
            $"Create{symbol.Name}Command.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateHandler(GeneratorExecutionContext context, ISymbol symbol)
    {
        var template = ReadTemplate(Configuration.CreateCommandCommandGenerator.CommandTemplatePath);

        var sourceCode = template.Render(new
        {
            EntityName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name,
            CommandName = $"Create{symbol.Name}Command",
        });

        context.AddSource(
            $"Create{symbol.Name}Handler.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class GenerateCreateCommandAttribute : Attribute
{
}