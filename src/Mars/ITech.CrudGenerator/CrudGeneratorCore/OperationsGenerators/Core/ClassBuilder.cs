using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;

internal class ClassBuilder
{
    private FileScopedNamespaceDeclarationSyntax? _namespace;
    private readonly List<string> _usings = [];
    private ClassDeclarationSyntax _classDeclaration;
    private readonly List<MemberDeclarationSyntax> _methods = new();

    public ClassBuilder(string className)
    {
        _classDeclaration = SyntaxFactory.ClassDeclaration(className)
            .AddModifiers([
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                SyntaxFactory.Token(SyntaxKind.PartialKeyword)
            ]);
    }

    public ClassBuilder WithNamespace(string @namespace)
    {
        _namespace = SyntaxFactory.FileScopedNamespaceDeclaration(SyntaxFactory.ParseName(@namespace));
        return this;
    }

    public ClassBuilder WithUsings(IEnumerable<string> usings)
    {
        _usings.AddRange(usings);
        return this;
    }

    public CompilationUnitSyntax Build()
    {
        var compilationUnit = SyntaxFactory.CompilationUnit();
        var usings = _usings.Select(x => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(x))).ToArray();
        compilationUnit = compilationUnit.AddUsings(usings);
        _classDeclaration = _classDeclaration.AddMembers(_methods.ToArray());
        _namespace = _namespace?.AddMembers(_classDeclaration);

        if (_namespace != null)
        {
            compilationUnit = compilationUnit.AddMembers(_namespace);
        }

        return compilationUnit;
    }

    public string BuildAsString()
    {
        // Normalize and get code as string.
        var result = Build()
            .NormalizeWhitespace()
            .ToFullString();

        return result;
    }

    public ClassBuilder WithMethod(MethodDeclarationSyntax endpointMethod)
    {
        _methods.Add(endpointMethod);
        return this;
    }

    public static ClassBuilder FromData(ClassBuilderData data)
    {
        return new ClassBuilder(data.ClassName)
            .WithNamespace(data.Namespace)
            .WithUsings(data.Usings)
            .WithMethod(data.Method.Build());
    }
}

internal class ClassBuilderData
{
    public EntityTitle EntityTitle { get; set; }
    public List<string> Usings { get; set; } = [];
    public string Namespace { get; set; }
    public string ClassName { get; set; }
    public MethodBuilder Method { get; set; }
}