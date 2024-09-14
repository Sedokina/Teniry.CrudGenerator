using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Mars.Generators.ApplicationGenerators.Core;

public abstract class BaseGenerator
{
    protected readonly GeneratorExecutionContext Context;
    protected readonly ISymbol Symbol;
    protected readonly CrudGeneratorConfiguration Configuration = CrudGeneratorConfiguration.Instance;

    protected BaseGenerator(GeneratorExecutionContext context, ISymbol symbol)
    {
        Context = context;
        Symbol = symbol;
    }

    protected void WriteFile(string templatePath, object model, string className)
    {
        var template = ReadTemplate(templatePath);
        var sourceCode = template.Render(model);

        Context.AddSource($"{className}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    private Template ReadTemplate(string templatePath)
    {
        return Template.Parse(GetEmbeddedResource(templatePath, GetType().Assembly));
    }

    private static string GetEmbeddedResource(string path, Assembly assembly)
    {
        using var stream = assembly.GetManifestResourceStream(path);
        using var streamReader = new StreamReader(stream ?? throw new InvalidOperationException());
        return streamReader.ReadToEnd();
    }
}