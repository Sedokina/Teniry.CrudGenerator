using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.Properties;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;

internal class MethodBodyBuilder
{
    private BlockSyntax _body = SyntaxFactory.Block();

    public MethodBodyBuilder InitVariableFromConstructorCall(
        string variableName,
        string className,
        List<EntityProperty> constructorArguments)
    {
        ExpressionSyntax initializationExpression = SyntaxFactory.ObjectCreationExpression(
            SyntaxFactory.Token(SyntaxKind.NewKeyword),
            SyntaxFactory.ParseTypeName(className),
            SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                constructorArguments.Select(x => SyntaxFactory.Argument(
                        SyntaxFactory.IdentifierName(x.PropertyNameAsMethodParameterName)))
                    .ToArray()
            )),
            null
        );

        // Initialize query variable with query object value
        var variableDeclarator = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(variableName), null,
            SyntaxFactory.EqualsValueClause(initializationExpression));
        var variableDeclaration = SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("var"))
            .WithVariables(SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>().Add(variableDeclarator));

        _body = _body.AddStatements(SyntaxFactory.LocalDeclarationStatement(variableDeclaration));
        return this;
    }

    public MethodBodyBuilder InitVariableFromGenericMethodCall(
        string variableName,
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<string> methodArgumentsAsVariableNames)
    {
        var dispatcherCall = SyntaxFactory.AwaitExpression(
            SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(objectWithMethod),
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(methodNameToCall))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SeparatedList<TypeSyntax>(
                                    methodGenericTypeNames.Select(SyntaxFactory.IdentifierName)
                                )
                            )
                        )
                ),
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                    methodArgumentsAsVariableNames
                        .Select(x => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x))).ToArray()
                ))
            )
        );

        var variableDeclaratorResultVariable = SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(variableName),
            null,
            SyntaxFactory.EqualsValueClause(dispatcherCall));
        var variableDeclarationResultVariable = SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("var"))
            .WithVariables(
                SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>().Add(variableDeclaratorResultVariable));

        _body = _body.AddStatements(SyntaxFactory.LocalDeclarationStatement(variableDeclarationResultVariable));
        return this;
    }

    public MethodBodyBuilder ReturnTypedResultOk(string variableName)
    {
        var returnStatement = SyntaxFactory.ReturnStatement(
            SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("TypedResults"),
                    SyntaxFactory.IdentifierName("Ok")),
                SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList([
                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName(variableName))
                ]))));
        _body = _body.AddStatements(returnStatement);
        return this;
    }

    public BlockSyntax Build()
    {
        return _body;
    }
}