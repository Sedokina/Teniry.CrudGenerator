using System;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Mars.Generators;

[Generator]
public class CreateCommandGenerator : ISourceGenerator
{
    private const string ResourcePath = "Mars.Generators.Templates.CreateHandler.txt"; 
    
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

            // Generate the real source code. Pass the template parameter if there is a overriden template.
            var sourceCode = GetSourceCodeFor(symbol);
            context.AddSource(
                $"Create{symbol.Name}Handler.g.cs",
                SourceText.From(sourceCode, Encoding.UTF8));
        }
    }
    
    private string GetSourceCodeFor(ISymbol symbol, string template = null)
    {
        // If template isn't provieded, use default one from embeded resources.
        template ??= GetEmbeddedResource(ResourcePath);

        // Can't use scriban at the moment, make it manually for now.
        return template
                .Replace("{{" + nameof(DefaultTemplateParameters.ClassName) + "}}", symbol.Name)
                .Replace("{{" + nameof(DefaultTemplateParameters.Namespace) + "}}", GetNamespaceRecursively(symbol.ContainingNamespace))
                .Replace("{{" + nameof(DefaultTemplateParameters.PrefferredNamespace) + "}}", symbol.ContainingAssembly.Name);
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