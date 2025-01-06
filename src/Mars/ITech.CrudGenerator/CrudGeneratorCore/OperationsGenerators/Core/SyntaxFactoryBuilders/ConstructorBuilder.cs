using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders;

internal class ConstructorBuilder
{
    private ConstructorDeclarationSyntax _constructorDeclaration;

    public ConstructorBuilder(string name)
    {
        _constructorDeclaration = ConstructorDeclaration(name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword));
    }

    public ConstructorBuilder WithParameters(List<ParameterOfMethodBuilder> properties)
    {
        _constructorDeclaration = _constructorDeclaration.AddParameterListParameters(
            properties.Select(
                x => Parameter(Identifier(x.Name)).WithType(ParseTypeName(x.Type))
            ).ToArray()
        );
        return this;
    }

    public ConstructorBuilder WithBody(MethodBodyBuilder body)
    {
        _constructorDeclaration = _constructorDeclaration.WithBody(body.Build());
        return this;
    }

    public ConstructorBuilder WithBaseConstructor(string[] argumentNames)
    {
        var baseArguments = ArgumentList(
            SeparatedList(argumentNames.Select(x =>
                Argument(IdentifierName(x)))));

        _constructorDeclaration = _constructorDeclaration.WithInitializer(
            ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, baseArguments));

        return this;
    }

    public ConstructorDeclarationSyntax Build()
    {
        return _constructorDeclaration;
    }
}