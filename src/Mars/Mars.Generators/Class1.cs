using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Mars.Generators;

[Generator]
public class ServiceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver<GenerateServiceAttribute>());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not AttributeSyntaxReceiver<GenerateServiceAttribute> syntaxReceiver)
        {
            return;
        }

        foreach (var classSyntax in syntaxReceiver.Classes)
        {
            // Converting the class to semantic model to access much more meaningful data.
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            // Parse to declared symbol, so you can access each part of code separately, such as interfaces, methods, members, contructor parameters etc.
            var symbol = model.GetDeclaredSymbol(classSyntax);

            // Finding my GenerateServiceAttribute over it. I'm sure this attribute is placed, because my syntax receiver already checked before.
            // So, I can surely execute following query.
            var attribute = classSyntax.AttributeLists.SelectMany(sm => sm.Attributes).First(x => x.Name.ToString().EnsureEndsWith("Attribute").Equals(typeof(GenerateServiceAttribute).Name));
            
            // Getting constructor parameter of the attribute. It might be not presented.
            var templateParameter = attribute.ArgumentList?.Arguments.FirstOrDefault()?.GetLastToken().ValueText; // Temprorary... Attribute has only one argument for now.

            // Can't access embeded resource of main project.
            // So overridden template must be marked as Analyzer Additional File to be able to be accessed by an analyzer.
            var overridenTemplate = templateParameter != null ?
                context.AdditionalFiles.FirstOrDefault(x => x.Path.EndsWith(templateParameter))?.GetText().ToString() :
                null;

            // Generate the real source code. Pass the template parameter if there is a overriden template.
            var sourceCode = GetSourceCodeFor(symbol, overridenTemplate);

            context.AddSource(
                $"{symbol.Name}{templateParameter ?? "Controller"}.g.cs",
                SourceText.From(sourceCode, Encoding.UTF8));
            Console.WriteLine(classSyntax);
        }
    }

    private string GetSourceCodeFor(ISymbol symbol, string template = null)
    {
        // If template isn't provieded, use default one from embeded resources.
        template ??= GetEmbededResource($"Mars.Generators.Templates.RepositoryController.txt");

        // Can't use scriban at the moment, make it manually for now.
        return template
            .Replace("{{" + nameof(DefaultTemplateParameters.ClassName) + "}}", symbol.Name)
            .Replace("{{" + nameof(DefaultTemplateParameters.Namespace) + "}}", GetNamespaceRecursively(symbol.ContainingNamespace))
            .Replace("{{" + nameof(DefaultTemplateParameters.PrefferredNamespace) + "}}", symbol.ContainingAssembly.Name)
            ;
    }

    private string GetEmbededResource(string path)
    {
        using var stream = GetType().Assembly.GetManifestResourceStream(path);

        using var streamReader = new StreamReader(stream);

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

public static class StringExtensions
{
    public static string EnsureEndsWith(
        this string source,
        string suffix)
    {
        if (source.EndsWith(suffix))
        {
            return source;
        }
        return source + suffix;
    }
}

public class AttributeSyntaxReceiver<TAttribute> : ISyntaxReceiver
    where TAttribute : Attribute
{
    public IList<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
            classDeclarationSyntax.AttributeLists.Count > 0 &&
            classDeclarationSyntax.AttributeLists
                .Any(al => al.Attributes
                    .Any(a => a.Name.ToString().EnsureEndsWith("Attribute").Equals(typeof(TAttribute).Name))))
        {
            Classes.Add(classDeclarationSyntax);
        }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class GenerateServiceAttribute : Attribute
{
    public GenerateServiceAttribute(string template = null)
    {
    }
}