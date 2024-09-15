using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using Scriban.Runtime;

namespace Mars.Generators.ApplicationGenerators.Core;

public abstract class BaseGenerator
{
    protected readonly CrudGeneratorConfiguration Configuration;
    protected readonly GeneratorExecutionContext Context;
    protected readonly ISymbol Symbol;
    protected readonly string _entityName;
    protected readonly string _putIntoNamespace;
    protected readonly string _usingEntityNamespace;


    protected BaseGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        CrudGeneratorConfiguration configuration,
        NameConfiguration functionNameConfiguration)
    {
        Configuration = configuration;
        Context = context;
        Symbol = symbol;
        _entityName = Symbol.Name;
        _usingEntityNamespace = Symbol.ContainingNamespace.ToString();
        _putIntoNamespace = Configuration.PutIntoNamespaceBasePath.GetNamespacePath(
            _entityName,
            Symbol.ContainingAssembly.Name,
            Configuration.FeatureNameConfiguration,
            functionNameConfiguration);
    }

    protected void WriteFile(string templatePath, object model, string className)
    {
        var template = ReadTemplate(templatePath);

        var customProps = new ScriptObject();
        customProps.Import(model);

        var baseProps = new ScriptObject();
        baseProps.Import(new
        {
            EntityName = _entityName,
            EntityNamespace = _usingEntityNamespace,
            PutIntoNamespace = _putIntoNamespace
        });

        var context = new TemplateContext();
        context.PushGlobal(customProps);
        context.PushGlobal(baseProps);
        var sourceCode = template.Render(context);

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