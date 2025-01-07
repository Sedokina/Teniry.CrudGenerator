using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders;

public class FluentCallBuilder
{
    private InvocationExpressionSyntax _call = null!;

    public FluentCallBuilder CallGenericMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<ExpressionSyntax> arguments)
    {
        _call = SimpleSyntaxFactory.CallGenericMethod(
            objectWithMethod,
            methodNameToCall,
            methodGenericTypeNames,
            arguments
        );
        return this;
    }

    public FluentCallBuilder ThenMethod(string methodNameToCall, List<ExpressionSyntax> arguments)
    {
        _call = _call.WithExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                _call,
                IdentifierName(Identifier(methodNameToCall))
            )
        ).WithArgumentList(ArgumentList(SeparatedList(arguments.Select(Argument).ToArray())));

        return this;
    }

    public FluentCallBuilder ThenGenericMethod(
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<ExpressionSyntax> arguments)
    {
        _call = _call.WithExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                _call,
                GenericName(Identifier(methodNameToCall))
                    .WithTypeArgumentList(
                        TypeArgumentList(SeparatedList<TypeSyntax>(
                                methodGenericTypeNames.Select(IdentifierName)
                            )
                        )
                    )
            )
        ).WithArgumentList(ArgumentList(SeparatedList(arguments.Select(Argument).ToArray())));

        return this;
    }

    public AwaitExpressionSyntax BuildAsyncCall()
    {
        return AwaitExpression(_call);
    }
}