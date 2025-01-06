using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;

public class PropertyBuilder
{
    private PropertyDeclarationSyntax _property;

    public PropertyBuilder(string fieldType, string fieldName)
    {
        _property = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(fieldType), fieldName)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
            .WithAccessorList(SyntaxFactory.AccessorList(
                SyntaxFactory.List([
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                ])));
    }

    public PropertyBuilder WithDefaultValue(string? defaultValue = null)
    {
        if (defaultValue is null)
        {
            return this;
        }

        _property = _property.WithInitializer(
                // TODO: set actual default value, when it would not be "\"\""
                SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("")))
            )
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        return this;
    }

    public PropertyBuilder WithInheritDoc()
    {
        _property = _property.WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia("/// <inheritdoc />\n"));
        return this;
    }

    public PropertyDeclarationSyntax Build()
    {
        return _property;
    }
}