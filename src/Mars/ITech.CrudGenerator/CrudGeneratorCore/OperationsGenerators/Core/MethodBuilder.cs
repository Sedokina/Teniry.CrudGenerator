using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;

internal class MethodBuilder
{
    private MethodDeclarationSyntax _methodDeclaration;

    public MethodBuilder(SyntaxKind[] modifiers, string returnType, string name)
    {
        var returnTypeSyntax = SyntaxFactory.ParseTypeName(returnType);
        _methodDeclaration = SyntaxFactory.MethodDeclaration(returnTypeSyntax, name)
            .AddModifiers(modifiers.Select(SyntaxFactory.Token)
                .ToArray());
    }

    public MethodBuilder WithParameters(List<ParameterOfMethodBuilder> properties)
    {
        _methodDeclaration = _methodDeclaration.AddParameterListParameters(properties
            .Select(x =>
            {
                var a = x.GetAsMethodParameter();
                return SyntaxFactory
                    .Parameter(SyntaxFactory.Identifier(a.Name))
                    .WithType(SyntaxFactory.ParseTypeName(a.Type));
            }).ToArray());
        return this;
    }
    
    
    public MethodBuilder WithXmlDoc(string summary, int responseStatusCode, string response)
    {
        var xmlDoc = @$"
/// <summary>
///     {summary}
/// </summary>
/// <response code=""{responseStatusCode}"">{response}</response>
";
        _methodDeclaration = _methodDeclaration.WithLeadingTrivia(SyntaxFactory.ParseLeadingTrivia(xmlDoc));
        return this;
    }

    public MethodBuilder WithBody(BlockSyntax body)
    {
        _methodDeclaration = _methodDeclaration.WithBody(body);
        return this;
    }

    public MethodBuilder WithProducesResponseTypeAttribute(string typeName, int statusCode = 200)
    {
        ProducesResponseTypeAttribute(typeName, statusCode);
        return this;
    }

    public MethodBuilder WithProducesResponseTypeAttribute(int statusCode = 200)
    {
        ProducesResponseTypeAttribute(null, statusCode);
        return this;
    }

    private MethodBuilder ProducesResponseTypeAttribute(string? typeName, int statusCode)
    {
        var arguments = new List<SyntaxNodeOrToken>();
        if (!string.IsNullOrEmpty(typeName))
        {
            arguments.AddRange(
            [
                SyntaxFactory.AttributeArgument(SyntaxFactory.TypeOfExpression(SyntaxFactory.IdentifierName(typeName))),
                SyntaxFactory.Token(SyntaxKind.CommaToken)
            ]);
        }

        arguments.Add(SyntaxFactory.AttributeArgument(
            SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(statusCode))));


        _methodDeclaration = _methodDeclaration.WithAttributeLists(SyntaxFactory.SingletonList(
            SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(
                            SyntaxFactory.IdentifierName("ProducesResponseType"))
                        .WithArgumentList(
                            SyntaxFactory.AttributeArgumentList(
                                SyntaxFactory.SeparatedList<AttributeArgumentSyntax>(arguments)))))
        ));
        return this;
    }

    public MethodDeclarationSyntax Build()
    {
        return _methodDeclaration;
    }
}

internal class ParameterOfMethodBuilder
{
    public string Type { get; set; }
    public string Name { get; set; }

    public ParameterOfMethodBuilder(string type, string name)
    {
        Type = type;
        Name = name;
    }

    public (string Type, string Name) GetAsMethodParameter()
    {
        return (Type, Name);
    }
}