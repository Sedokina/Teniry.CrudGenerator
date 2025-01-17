using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.Core.Generators.Core.SyntaxFactoryBuilders;

internal class MethodBuilder {
    private MethodDeclarationSyntax _methodDeclaration;

    public MethodBuilder(SyntaxKind[] modifiers, string returnType, string name) {
        _methodDeclaration = MethodDeclaration(ParseTypeName(returnType), name)
            .AddModifiers(modifiers.Select(Token).ToArray());
    }

    public MethodBuilder WithParameters(List<ParameterOfMethodBuilder> properties) {
        _methodDeclaration = _methodDeclaration.AddParameterListParameters(
            properties.Select(
                x => Parameter(Identifier(x.Name))
                    .WithType(ParseTypeName(x.Type))
                    .WithModifiers(TokenList(x.Modifiers.Select(Token)))
            ).ToArray()
        );

        return this;
    }

    public MethodBuilder WithXmlDoc(string summary, int responseStatusCode, string response) {
        var xmlDoc = @$"
/// <summary>
///     {summary}
/// </summary>
/// <response code=""{responseStatusCode}"">{response}</response>
";
        _methodDeclaration = _methodDeclaration.WithLeadingTrivia(ParseLeadingTrivia(xmlDoc));

        return this;
    }

    public MethodBuilder WithXmlInheritdoc() {
        _methodDeclaration = _methodDeclaration.WithLeadingTrivia(ParseLeadingTrivia("/// <inheritdoc />\n"));

        return this;
    }

    public MethodBuilder WithAttribute(ProducesResponseTypeAttributeBuilder attribute) {
        _methodDeclaration = _methodDeclaration
            .AddAttributeLists(AttributeList(SingletonSeparatedList(attribute.Build())));

        return this;
    }

    public MethodBuilder WithBody(BlockBuilder body) {
        _methodDeclaration = _methodDeclaration.WithBody(body.Build());

        return this;
    }

    public MethodBuilder WithBody(BlockSyntax body) {
        _methodDeclaration = _methodDeclaration.WithBody(body);

        return this;
    }

    public MethodDeclarationSyntax Build() {
        return _methodDeclaration;
    }
}