using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using Scriban.Runtime;

namespace Mars.Generators.ApplicationGenerators.Core;

public abstract class BaseGenerator<TConfiguration> where TConfiguration : IQueryCommandGeneratorConfiguration
{
    protected readonly TConfiguration Configuration;
    protected readonly GeneratorExecutionContext Context;
    protected readonly ISymbol Symbol;
    protected readonly string EntityName;
    protected readonly string PutIntoNamespace;
    protected readonly string UsingEntityNamespace;
    public string EndpointMapCall { get; set; }

    protected BaseGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        TConfiguration configuration)
    {
        Configuration = configuration;
        Context = context;
        Symbol = symbol;
        EntityName = Symbol.Name;
        UsingEntityNamespace = Symbol.ContainingNamespace.ToString();
        PutIntoNamespace = Configuration.FullConfiguration.PutIntoNamespaceBasePath.GetNamespacePath(
            EntityName,
            Symbol.ContainingAssembly.Name,
            Configuration.FullConfiguration.FeatureNameConfiguration,
            configuration.FunctionNameConfiguration);
    }

    protected void WriteFile(string templatePath, object model, string className)
    {
        var template = ReadTemplate(templatePath);

        var baseProps = new ScriptObject();
        baseProps.Import(new
        {
            EntityName = EntityName,
            EntityNamespace = UsingEntityNamespace,
            PutIntoNamespace = PutIntoNamespace
        });

        var customProps = new ScriptObject();
        customProps.Import(model);

        var context = new TemplateContext();
        context.PushGlobal(baseProps);
        context.PushGlobal(customProps);
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