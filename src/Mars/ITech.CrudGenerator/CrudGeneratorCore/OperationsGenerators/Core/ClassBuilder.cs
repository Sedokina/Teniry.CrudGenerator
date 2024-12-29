using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;

internal class ClassBuilder
{
    private FileScopedNamespaceDeclarationSyntax? _namespace;
    private readonly List<string> _usings = [];
    private readonly List<BaseTypeSyntax> _implementInterfaces = [];
    private ClassDeclarationSyntax _classDeclaration;
    private readonly List<MemberDeclarationSyntax> _methods = new();
    private readonly List<MemberDeclarationSyntax> _fields = new();

    public ClassBuilder(SyntaxKind[] modifiers, string className)
    {
        _classDeclaration = SyntaxFactory.ClassDeclaration(className)
            .AddModifiers(modifiers.Select(SyntaxFactory.Token).ToArray());
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

    public ClassBuilder Implements(string interfaceName, params string[] interfaceParams)
    {
        var addInterface = interfaceName;
        if (interfaceParams.Length > 0)
        {
            var newParams = string.Join(", ", interfaceParams);
            addInterface = $"{addInterface}<{newParams}>";
        }

        _implementInterfaces.Add(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(addInterface)));

        return this;
    }


    public ClassBuilder WithPrivateField(SyntaxKind[] modifiers, string fieldType, string fieldName)
    {
        var field = SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName(fieldType))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                SyntaxFactory.Identifier(fieldName)))))
            .WithModifiers(
                SyntaxFactory.TokenList(modifiers.Select(SyntaxFactory.Token).ToArray()));
        _fields.Add(field);
        return this;
    }

    public CompilationUnitSyntax Build()
    {
        var compilationUnit = SyntaxFactory.CompilationUnit();
        var usings = _usings.Select(x => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(x))).ToArray();
        compilationUnit = compilationUnit.AddUsings(usings);
        // _classDeclaration = _classDeclaration.AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName("IDisposable")));

        if (_implementInterfaces.Count > 0)
        {
            _classDeclaration = _classDeclaration.AddBaseListTypes(_implementInterfaces.ToArray());
        }

        _classDeclaration = _classDeclaration.AddMembers(_fields.ToArray());
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
}