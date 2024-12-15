using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;

internal abstract class BaseGenerator
{
    public List<GeneratorResult> GeneratedFiles { get; protected set; } = [];
    protected readonly string AutogeneratedFileText;
    protected readonly bool NullableEnable;

    protected BaseGenerator(string autogeneratedFileText, bool nullableEnable)
    {
        AutogeneratedFileText = autogeneratedFileText;
        NullableEnable = nullableEnable;
    }

    public abstract void RunGenerator();

    protected virtual void WriteFile(string templatePath, object model, string className)
    {
        var template = ReadTemplate(templatePath);

        var sb = new StringBuilder();
        sb.AppendLine(AutogeneratedFileText);
        if (NullableEnable)
        {
            sb.AppendLine("#nullable enable");
        }

        sb.AppendLine(template.Render(model));

        var sourceCode = sb.ToString().Trim();
        var result = CSharpSyntaxTree.ParseText(SourceText.From(sourceCode, Encoding.UTF8))
            .GetRoot()
            .NormalizeWhitespace()
            .SyntaxTree
            .GetText();

        GeneratedFiles.Add(new GeneratorResult($"{className}.g.cs", result));
    }

    protected virtual void WriteFile(string className, string code)
    {
        var sb = new StringBuilder();
        sb.AppendLine(AutogeneratedFileText);
        if (NullableEnable)
        {
            sb.AppendLine("#nullable enable");
        }

        sb.AppendLine(code);

        var sourceCode = sb.ToString().Trim();

        var result = CSharpSyntaxTree.ParseText(SourceText.From(sourceCode, Encoding.UTF8))
            .GetRoot()
            .NormalizeWhitespace()
            .SyntaxTree
            .GetText();

        GeneratedFiles.Add(new GeneratorResult($"{className}.g.cs", result));
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

public class GeneratorResult
{
    public string FileName { get; }
    public SourceText Source { get; }

    public GeneratorResult(string fileName, SourceText source)
    {
        FileName = fileName;
        Source = source;
    }
}