using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders;

public class ExpressionBuilder
{
    private ExpressionSyntax _statement;

    public ExpressionBuilder CallConstructor(
        string className,
        List<string> constructorArguments)
    {
        _statement = ObjectCreationExpression(
            Token(SyntaxKind.NewKeyword),
            ParseTypeName(className),
            ArgumentList(SeparatedList(
                constructorArguments.Select(x => Argument(IdentifierName(x))).ToArray()
            )),
            null
        );
        return this;
    }

    public ExpressionBuilder CallMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodArgumentsAsVariableNames)
    {
        _statement = InvocationExpression(
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

        return this;
    }

    public ExpressionBuilder CallGenericMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<string> methodArgumentsAsVariableNames)
    {
        _statement = InvocationExpression(
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
        return this;
    }

    public ExpressionBuilder CallGenericAsyncMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<string> methodArgumentsAsVariableNames)
    {
        _statement = AwaitExpression(
            InvocationExpression(
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
                    methodArgumentsAsVariableNames.Select(x => Argument(IdentifierName(x))).ToArray()
                ))
            )
        );
        return this;
    }

    public ExpressionBuilder CallAsyncMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodArgumentsAsVariableNames)
    {
        _statement = AwaitExpression(
            InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(objectWithMethod),
                    IdentifierName(Identifier(methodNameToCall))
                ),
                ArgumentList(SeparatedList(
                    methodArgumentsAsVariableNames
                        .Select(x => Argument(IdentifierName(x))).ToArray()
                ))
            )
        );
        return this;
    }

    public ExpressionBuilder WithAsyncLinq(LinqCallBuilder linqCallBuilder)
    {
        _statement = AwaitExpression(linqCallBuilder.Build());
        return this;
    }

    public ExpressionBuilder Variable(string variableName)
    {
        _statement = IdentifierName(variableName);
        return this;
    }

    public ExpressionBuilder InterpolatedString(string interpolatedString)
    {
        _statement = InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
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
        return this;
    }

    public ExpressionBuilder NewArray(string typeName, IEnumerable<string> parameters)
    {
        var arrayType = ArrayType(ParseTypeName(typeName))
            .WithRankSpecifiers(SingletonList(
                ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))));

        _statement = ArrayCreationExpression(arrayType)
            .WithInitializer(InitializerExpression(
                SyntaxKind.ArrayInitializerExpression,
                SeparatedList<ExpressionSyntax>(
                    parameters.Select(IdentifierName).ToArray()
                )
            ));
        return this;
    }

    public ExpressionBuilder NewStringLiteralArray(IEnumerable<string> parameters)
    {
        var arrayType = ArrayType(ParseTypeName("string"))
            .WithRankSpecifiers(SingletonList(
                ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))));

        _statement = ArrayCreationExpression(arrayType)
            .WithInitializer(InitializerExpression(
                SyntaxKind.ArrayInitializerExpression,
                SeparatedList<ExpressionSyntax>(
                    parameters.Select(p => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(p))).ToArray()
                )
            ));
        return this;
    }

    public ExpressionSyntax Build()
    {
        return _statement;
    }
}

// public class StatementBuilder
// {
//     private StatementSyntax _statement;
//
//     public StatementBuilder IfNull(string variableName)
//     {
//         _statement = IfStatement(ParseExpression($"{variableName} == null"),
//             Block(
//             )
//         );
//         return this;
//     }
//
//     public StatementBuilder Build()
//     {
//         return this;
//     }
// }

internal class BlockBuilder
{
    private BlockSyntax _body = Block();

    public BlockBuilder InitVariable(string variableName,
        Func<ExpressionBuilder, ExpressionBuilder> expressionBuilderFunc)
    {
        var statement = expressionBuilderFunc(new ExpressionBuilder()).Build();
        var variableDeclarator = VariableDeclarator(Identifier(variableName), null,
            EqualsValueClause(statement));
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
        var statementBuilder = new ExpressionBuilder()
            .CallMethod(objectWithMethod, methodNameToCall, methodArgumentsAsVariableNames);

        _body = _body.AddStatements(ExpressionStatement(statementBuilder.Build()));
        return this;
    }

    public BlockBuilder CallAsyncMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodArgumentsAsVariableNames)
    {
        var statementBuilder = new ExpressionBuilder()
            .CallAsyncMethod(objectWithMethod, methodNameToCall, methodArgumentsAsVariableNames);

        _body = _body.AddStatements(ExpressionStatement(statementBuilder.Build()));
        return this;
    }

    public BlockBuilder CallGenericAsyncMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodGenericTypeNames,
        List<string> methodArgumentsAsVariableNames)
    {
        var statementBuilder = new ExpressionBuilder()
            .CallGenericAsyncMethod(
                objectWithMethod,
                methodNameToCall,
                methodGenericTypeNames,
                methodArgumentsAsVariableNames
            );
        _body = _body.AddStatements(ExpressionStatement(statementBuilder.Build()));
        return this;
    }

    public BlockBuilder Return()
    {
        _body = _body.AddStatements(ReturnStatement());
        return this;
    }

    public BlockBuilder Return(Func<ExpressionBuilder, ExpressionBuilder> expressionBuilderFunc)
    {
        var statement = expressionBuilderFunc(new ExpressionBuilder()).Build();
        _body = _body.AddStatements(ReturnStatement(statement));
        return this;
    }

    public BlockSyntax Build()
    {
        return _body;
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
        _call =
            InvocationExpression(
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

        return this;
    }

    public LinqCallBuilder ThenMethod(
        string methodNameToCall,
        List<string> methodArgumentsAsVariableNames)
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