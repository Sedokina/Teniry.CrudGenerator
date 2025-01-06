using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders;

public class ProducesResponseTypeAttributeBuilder
{
    private readonly AttributeSyntax _attribute;

    public ProducesResponseTypeAttributeBuilder(string typeName, int statusCode = 200)
    {
        _attribute = ProducesResponseTypeAttribute(typeName, statusCode);
    }

    public ProducesResponseTypeAttributeBuilder(int statusCode = 200)
    {
        _attribute = ProducesResponseTypeAttribute(null, statusCode);
    }

    private AttributeSyntax ProducesResponseTypeAttribute(string? typeName, int statusCode)
    {
        var arguments = new List<SyntaxNodeOrToken>();
        if (!string.IsNullOrEmpty(typeName))
        {
            arguments.AddRange(
            [
                SyntaxFactory.AttributeArgument(
                    SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(typeName!))),
                SyntaxFactory.Token(SyntaxKind.CommaToken)
            ]);
        }

        arguments.Add(SyntaxFactory.AttributeArgument(
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(statusCode))));


        return SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("ProducesResponseType"))
            .WithArgumentList(
                SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(arguments)));
    }

    public AttributeSyntax Build()
    {
        return _attribute;
    }
}