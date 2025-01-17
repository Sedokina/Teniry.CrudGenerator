using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Teniry.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders;

public class ProducesResponseTypeAttributeBuilder {
    private readonly AttributeSyntax _attribute;

    public ProducesResponseTypeAttributeBuilder(string typeName, int statusCode = 200) {
        _attribute = ProducesResponseTypeAttribute(typeName, statusCode);
    }

    public ProducesResponseTypeAttributeBuilder(int statusCode = 200) {
        _attribute = ProducesResponseTypeAttribute(null, statusCode);
    }

    private AttributeSyntax ProducesResponseTypeAttribute(string? typeName, int statusCode) {
        var arguments = new List<SyntaxNodeOrToken>();
        if (!string.IsNullOrEmpty(typeName)) {
            arguments.AddRange(
                [
                    AttributeArgument(TypeOfExpression(IdentifierName(typeName!))),
                    Token(SyntaxKind.CommaToken)
                ]
            );
        }

        arguments.Add(AttributeArgument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(statusCode))));

        return Attribute(IdentifierName("ProducesResponseType"))
            .WithArgumentList(AttributeArgumentList(SeparatedList<AttributeArgumentSyntax>(arguments)));
    }

    public AttributeSyntax Build() {
        return _attribute;
    }
}