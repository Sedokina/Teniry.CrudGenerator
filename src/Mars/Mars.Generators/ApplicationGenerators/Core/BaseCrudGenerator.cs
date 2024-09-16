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
    protected readonly GeneratorExecutionContext Context;

    protected BaseGenerator(GeneratorExecutionContext context)
    {
        Context = context;
    }

    public abstract void RunGenerator();

    protected virtual void WriteFile(string templatePath, object model, string className)
    {
        var template = ReadTemplate(templatePath);

        var sourceCode = template.Render(model);

        Context.AddSource($"{className}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }

    internal Template ReadTemplate(string templatePath)
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

public abstract class BaseCrudGenerator<TConfiguration> : BaseGenerator
    where TConfiguration : IQueryCommandGeneratorConfiguration
{
    protected readonly TConfiguration Configuration;
    protected readonly ISymbol Symbol;
    protected readonly string EntityName;
    protected readonly string BusinessLogicNamespace;
    public string EndpointNamespace { get; set; }
    protected readonly string UsingEntityNamespace;
    public EndpointMap EndpointMap { get; set; }

    protected BaseCrudGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        TConfiguration configuration) : base(context)
    {
        Configuration = configuration;
        Symbol = symbol;
        EntityName = Symbol.Name;
        UsingEntityNamespace = Symbol.ContainingNamespace.ToString();
        BusinessLogicNamespace = Configuration.FullConfiguration.BusinessLogicNamespaceBasePath.GetNamespacePath(
            EntityName,
            Symbol.ContainingAssembly.Name,
            Configuration.FullConfiguration.FeatureNameConfiguration,
            configuration.FunctionNameConfiguration);
        EndpointNamespace = Configuration.FullConfiguration.EndpointsNamespaceBasePath.GetNamespacePath(
            EntityName,
            Symbol.ContainingAssembly.Name);
    }
    
    protected override void WriteFile(string templatePath, object model, string className)
    {
        var template = ReadTemplate(templatePath);

        var baseProps = new ScriptObject();
        baseProps.Import(new
        {
            EntityName = EntityName,
            EntityNamespace = UsingEntityNamespace,
            BusinessLogicNamespace = BusinessLogicNamespace,
            EndpointNamespace = EndpointNamespace
        });

        var customProps = new ScriptObject();
        customProps.Import(model);

        var context = new TemplateContext();
        context.PushGlobal(baseProps);
        context.PushGlobal(customProps);
        var sourceCode = template.Render(context);

        Context.AddSource($"{className}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}

public class EndpointMap
{
    public string EntityName { get; set; }
    public string EndpointNamespace { get; set; }
    public string HttpMethod { get; }
    public string EndpointRoute { get; set; }
    public string FunctionCall { get; set; }

    public EndpointMap(string entityName, string endpointNamespace, string httpMethod, string endpointRoute,
        string functionCall)
    {
        EntityName = entityName;
        EndpointNamespace = endpointNamespace;
        HttpMethod = httpMethod;
        EndpointRoute = endpointRoute;
        FunctionCall = functionCall;
    }
}