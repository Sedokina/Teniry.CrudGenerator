using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders;

public class PropertyBuilder
{
    private PropertyDeclarationSyntax _property;

    public PropertyBuilder(string fieldType, string fieldName)
    {
        _property = PropertyDeclaration(ParseTypeName(fieldType), fieldName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .WithAccessorList(AccessorList(
                List([
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
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
                EqualsValueClause(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("")))
            )
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        return this;
    }

    public PropertyBuilder WithInheritDoc()
    {
        _property = _property.WithLeadingTrivia(ParseLeadingTrivia("/// <inheritdoc />\n"));
        return this;
    }

    public PropertyDeclarationSyntax Build()
    {
        return _property;
    }
}