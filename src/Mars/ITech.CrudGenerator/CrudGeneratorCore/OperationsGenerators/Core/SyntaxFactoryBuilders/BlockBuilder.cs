using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders;

public static class SimpleSyntaxFactory
{
    public static ObjectCreationExpressionSyntax CallConstructor(
        string className,
        List<string> constructorArguments)
    {
        return ObjectCreationExpression(
            Token(SyntaxKind.NewKeyword),
            ParseTypeName(className),
            ArgumentList(SeparatedList(
                constructorArguments.Select(x => Argument(IdentifierName(x))).ToArray()
            )),
            null
        );
    }

    public static InvocationExpressionSyntax CallMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodArgumentsAsVariableNames)
    {
        return InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(objectWithMethod),
                IdentifierName(Identifier(methodNameToCall))
            ),
            ArgumentList(SeparatedList(
                methodArgumentsAsVariableNames
                    .Select(x => Argument(IdentifierName(x))).ToArray()
            ))
        );
    }

    public static AwaitExpressionSyntax CallAsyncMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodArgumentsAsVariableNames)
    {
        return AwaitExpression(CallMethod(objectWithMethod, methodNameToCall, methodArgumentsAsVariableNames));
    }

    public static InvocationExpressionSyntax CallGenericMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<string> methodArgumentsAsVariableNames)
    {
        return InvocationExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(objectWithMethod),
                GenericName(Identifier(methodNameToCall))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SeparatedList<TypeSyntax>(
                                methodGenericTypeNames.Select(IdentifierName)
                            )
                        )
                    )
            ),
            ArgumentList(SeparatedList(
                methodArgumentsAsVariableNames
                    .Select(x => Argument(IdentifierName(x))).ToArray()
            ))
        );
    }

    public static AwaitExpressionSyntax CallGenericAsyncMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<string> methodArgumentsAsVariableNames)
    {
        return AwaitExpression(
            CallGenericMethod(
                objectWithMethod,
                methodNameToCall,
                methodGenericTypeNames,
                methodArgumentsAsVariableNames
            )
        );
    }

    public static IdentifierNameSyntax Variable(string variableName)
    {
        return IdentifierName(variableName);
    }

    public static InterpolatedStringExpressionSyntax InterpolatedString(string interpolatedString)
    {
        return InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
            .WithContents(
                SingletonList<InterpolatedStringContentSyntax>(
                    InterpolatedStringText().WithTextToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.InterpolatedStringTextToken,
                            interpolatedString,
                            interpolatedString,
                            TriviaList()
                        )
                    )
                )
            );
    }

    public static ArrayCreationExpressionSyntax NewArray(string typeName, IEnumerable<string> parameters)
    {
        var arrayType = ArrayType(ParseTypeName(typeName))
            .WithRankSpecifiers(SingletonList(
                ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))));

        return ArrayCreationExpression(arrayType)
            .WithInitializer(InitializerExpression(
                SyntaxKind.ArrayInitializerExpression,
                SeparatedList<ExpressionSyntax>(
                    parameters.Select(IdentifierName).ToArray()
                )
            ));
    }

    public static ArrayCreationExpressionSyntax NewStringLiteralArray(IEnumerable<string> parameters)
    {
        var arrayType = ArrayType(ParseTypeName("string"))
            .WithRankSpecifiers(SingletonList(
                ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))));

        return ArrayCreationExpression(arrayType)
            .WithInitializer(InitializerExpression(
                SyntaxKind.ArrayInitializerExpression,
                SeparatedList<ExpressionSyntax>(
                    parameters.Select(p => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(p))).ToArray()
                )
            ));
    }

    public static AwaitExpressionSyntax WithAsyncLinq(LinqCallBuilder linqCallBuilder)
    {
        return AwaitExpression(linqCallBuilder.Build());
    }
}

internal class BlockBuilder
{
    private BlockSyntax _body = Block();

    public BlockBuilder InitVariable(
        string variableName,
        ExpressionSyntax expressionSyntax
    )
    {
        var variableDeclarator = VariableDeclarator(Identifier(variableName), null,
            EqualsValueClause(expressionSyntax));
        var variableDeclaration = VariableDeclaration(ParseTypeName("var"))
            .WithVariables(SeparatedList<VariableDeclaratorSyntax>().Add(variableDeclarator));
        _body = _body.AddStatements(LocalDeclarationStatement(variableDeclaration));
        return this;
    }

    public BlockBuilder CallMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodArgumentsAsVariableNames)
    {
        var statementBuilder = SimpleSyntaxFactory.CallMethod(objectWithMethod, methodNameToCall, methodArgumentsAsVariableNames);

        _body = _body.AddStatements(ExpressionStatement(statementBuilder));
        return this;
    }

    public BlockBuilder CallAsyncMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodArgumentsAsVariableNames)
    {
        var statementBuilder = SimpleSyntaxFactory.CallAsyncMethod(objectWithMethod, methodNameToCall, methodArgumentsAsVariableNames);

        _body = _body.AddStatements(ExpressionStatement(statementBuilder));
        return this;
    }

    public BlockBuilder CallGenericAsyncMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<string> methodArgumentsAsVariableNames)
    {
        var statementBuilder = SimpleSyntaxFactory.CallGenericAsyncMethod(
            objectWithMethod,
            methodNameToCall,
            methodGenericTypeNames,
            methodArgumentsAsVariableNames
        );
        _body = _body.AddStatements(ExpressionStatement(statementBuilder));
        return this;
    }

    public BlockBuilder Return()
    {
        _body = _body.AddStatements(ReturnStatement());
        return this;
    }

    public BlockBuilder Return(ExpressionSyntax expressionSyntax)
    {
        _body = _body.AddStatements(ReturnStatement(expressionSyntax));
        return this;
    }

    public BlockBuilder AssignVariable(string assignTo, string from)
    {
        var assignmentExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(assignTo),
            IdentifierName(from)
        );
        _body = _body.AddStatements(ExpressionStatement(assignmentExpression));
        return this;
    }

    public BlockBuilder IfNull(string variableName, Func<BlockBuilder, BlockBuilder> bodyBuilderFunc)
    {
        var ifBody = new BlockBuilder();
        bodyBuilderFunc(ifBody);
        var ifStatement = IfStatement(ParseExpression($"{variableName} == null"), ifBody.Build());
        _body = _body.AddStatements(ifStatement);
        return this;
    }

    public BlockBuilder ThrowEntityNotFoundException(string entityTypeName)
    {
        var throwStatement = ThrowStatement(ObjectCreationExpression(ParseTypeName("EfEntityNotFoundException"))
            .WithArgumentList(
                ArgumentList(
                    SingletonSeparatedList(
                        Argument(
                            TypeOfExpression(ParseTypeName(entityTypeName))
                        )
                    )
                )
            )
        );
        _body = _body.AddStatements(throwStatement);
        return this;
    }

    public void AddExpression(StatementSyntax expression)
    {
        _body = _body.AddStatements(expression);
    }
    
    public BlockSyntax Build()
    {
        return _body;
    }
}

public class LinqCallBuilder
{
    private InvocationExpressionSyntax _call = null!;

    public LinqCallBuilder CallGenericMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<string> methodArgumentsAsVariableNames)
    {
        _call = SimpleSyntaxFactory.CallGenericMethod(
            objectWithMethod,
            methodNameToCall,
            methodGenericTypeNames,
            methodArgumentsAsVariableNames
        );
        return this;
    }

    public LinqCallBuilder ThenMethod(string methodNameToCall, List<string> methodArgumentsAsVariableNames)
    {
        _call = _call.WithExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                _call,
                IdentifierName(Identifier(methodNameToCall))
            )
        ).WithArgumentList(ArgumentList(SeparatedList(
            methodArgumentsAsVariableNames
                .Select(x => Argument(IdentifierName(x))).ToArray()
        )));

        return this;
    }

    public LinqCallBuilder ThenGenericMethod(
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<string> methodArgumentsAsVariableNames)
    {
        _call = _call.WithExpression(
            MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                _call,
                GenericName(Identifier(methodNameToCall))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SeparatedList<TypeSyntax>(
                                methodGenericTypeNames.Select(IdentifierName)
                            )
                        )
                    )
            )
        ).WithArgumentList(ArgumentList(SeparatedList(
            methodArgumentsAsVariableNames
                .Select(x => Argument(IdentifierName(x))).ToArray()
        )));

        return this;
    }

    public InvocationExpressionSyntax Build()
    {
        return _call;
    }
}