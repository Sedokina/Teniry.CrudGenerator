using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;

internal class ConstructorBuilder
{
    private ConstructorDeclarationSyntax _constructorDeclaration;

    public ConstructorBuilder(SyntaxKind[] modifiers, string name)
    {
        _constructorDeclaration = SyntaxFactory
            .ConstructorDeclaration(name)
            .AddModifiers(modifiers.Select(SyntaxFactory.Token).ToArray());
    }

    public ConstructorBuilder WithParameters(List<ParameterOfMethodBuilder> properties)
    {
        _constructorDeclaration = _constructorDeclaration.AddParameterListParameters(properties
            .Select(x =>
            {
                var a = x.GetAsMethodParameter();
                return SyntaxFactory
                    .Parameter(SyntaxFactory.Identifier(a.Name))
                    .WithType(SyntaxFactory.ParseTypeName(a.Type));
            }).ToArray());
        return this;
    }

    public ConstructorBuilder WithBody(BlockSyntax body)
    {
        _constructorDeclaration = _constructorDeclaration.WithBody(body);
        return this;
    }

    public ConstructorDeclarationSyntax Build()
    {
        return _constructorDeclaration;
    }

    public ConstructorBuilder WithBaseConstructor(string[] argumentNames)
    {
        var baseArguments = SyntaxFactory.ArgumentList(
            SyntaxFactory.SeparatedList(argumentNames.Select(x =>
                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x)))));

        _constructorDeclaration = _constructorDeclaration.WithInitializer(
            SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, baseArguments));

        return this;
    }
}