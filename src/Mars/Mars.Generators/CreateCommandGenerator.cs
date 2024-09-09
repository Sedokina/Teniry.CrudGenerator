using System;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Mars.Generators;

[Generator]
public class CreateCommandGenerator : ISourceGenerator
{
    private const string CommandResourcePath = "Mars.Generators.Templates.CreateCommand.txt";
    private const string HandlerResourcePath = "Mars.Generators.Templates.CreateHandler.txt";

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

            GenerateHandler(context, symbol);
            GenerateCommand(context, symbol);
        }
    }

    private void GenerateCommand(GeneratorExecutionContext context, ISymbol symbol)
    {
        // Generate the real source code. Pass the template parameter if there is a overriden template.
        var template = Template.Parse(GetEmbeddedResource(CommandResourcePath));

        var sourceCode = template.Render(new
        {
            ClassName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name
        });

        context.AddSource(
            $"Create{symbol.Name}Command.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }

    private void GenerateHandler(GeneratorExecutionContext context, ISymbol symbol)
    {
        // Generate the real source code. Pass the template parameter if there is a overriden template.
        var template = Template.Parse(GetEmbeddedResource(HandlerResourcePath));

        var sourceCode = template.Render(new
        {
            ClassName = symbol.Name,
            Namespace = symbol.ContainingNamespace,
            PreferredNamespace = symbol.ContainingAssembly.Name,
            CommandName = $"Create{symbol.Name}Command"
        });

        context.AddSource(
            $"Create{symbol.Name}Handler.g.cs",
            SourceText.From(sourceCode, Encoding.UTF8));
    }


    private string GetEmbeddedResource(string path)
    {
        using var stream = GetType().Assembly.GetManifestResourceStream(path);

        using var streamReader = new StreamReader(stream ?? throw new InvalidOperationException());

        return streamReader.ReadToEnd();
    }

    private string GetNamespaceRecursively(INamespaceSymbol symbol)
    {
        if (symbol.ContainingNamespace == null)
        {
            return symbol.Name;
        }

        return (GetNamespaceRecursively(symbol.ContainingNamespace) + "." + symbol.Name).Trim('.');
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class GenerateCreateCommandAttribute : Attribute
{
    public GenerateCreateCommandAttribute()
    {
    }
}