using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;

internal class ClassBuilder
{
    private FileScopedNamespaceDeclarationSyntax? _namespace;
    private readonly List<string> _usings = [];
    private readonly List<BaseTypeSyntax> _implementInterfaces = [];
    private ClassDeclarationSyntax _classDeclaration;
    private readonly List<MemberDeclarationSyntax> _methods = [];
    private readonly List<MemberDeclarationSyntax> _fields = [];
    private readonly List<MemberDeclarationSyntax> _properties = [];
    private SyntaxTriviaList _xmlDoc;

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

    public ClassBuilder WithProperty(
        string fieldType,
        string fieldName,
        string? defaultValue = null,
        bool inheritdoc = false)
    {
        var property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(fieldType), fieldName)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .WithAccessorList(SyntaxFactory.AccessorList(
                SyntaxFactory.List([
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                ])));

        if (defaultValue is not null)
        {
            property = property.WithInitializer(SyntaxFactory.EqualsValueClause(
                    SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal("")))) // TODO: set actual default value, when it would not be "\"\""
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        }

        if (inheritdoc)
        {
            property = property.WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia("/// <inheritdoc />\n"));
        }

        _properties.Add(property);
        return this;
    }

    public CompilationUnitSyntax Build()
    {
        var compilationUnit = SyntaxFactory.CompilationUnit();
        var usings = _usings.Select(x => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(x))).ToArray();
        compilationUnit = compilationUnit.AddUsings(usings);

        if (_implementInterfaces.Count > 0)
        {
            _classDeclaration = _classDeclaration.AddBaseListTypes(_implementInterfaces.ToArray());
        }

        _classDeclaration = _classDeclaration.AddMembers(_fields.ToArray());
        _classDeclaration = _classDeclaration.AddMembers(_properties.ToArray());
        _classDeclaration = _classDeclaration.AddMembers(_methods.ToArray());
        _classDeclaration = _classDeclaration.WithLeadingTrivia(_xmlDoc);
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

    public ClassBuilder WithConstructor(ConstructorDeclarationSyntax constructorDeclaration)
    {
        _methods.Add(constructorDeclaration);
        return this;
    }

    public ClassBuilder WithXmlDoc(string summary, string returns = "", ResultException[]? exceptions = null)
    {
        var xmlDoc = new StringBuilder();
        xmlDoc.AppendLine(@$"
/// <summary>
///     {summary}
/// </summary>");
        if (!string.IsNullOrEmpty(returns))
        {
            xmlDoc.AppendLine($"/// <returns>{returns}</returns>");
        }

        if (exceptions is not null)
        {
            foreach (var exception in exceptions)
            {
                xmlDoc.AppendLine($"/// <exception cref=\"{exception.TypeName}\">{exception.Description}</exception>");
            }
        }

        _xmlDoc = SyntaxFactory.ParseLeadingTrivia(xmlDoc.ToString());
        return this;
    }
}

internal class ResultException(string typeName, string description)
{
    public string TypeName { get; set; } = typeName;
    public string Description { get; set; } = description;
}