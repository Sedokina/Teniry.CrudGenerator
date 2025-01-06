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

internal class MethodBodyBuilder
{
    private BlockSyntax _body = Block();

    public MethodBodyBuilder InitVariable(string variableName,
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

    public MethodBodyBuilder CallMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodArgumentsAsVariableNames)
    {
        var statementBuilder = new ExpressionBuilder()
            .CallMethod(objectWithMethod, methodNameToCall, methodArgumentsAsVariableNames);

        _body = _body.AddStatements(ExpressionStatement(statementBuilder.Build()));
        return this;
    }

    public MethodBodyBuilder CallAsyncMethod(
        string objectWithMethod,
        string methodNameToCall,
        List<string> methodArgumentsAsVariableNames)
    {
        var statementBuilder = new ExpressionBuilder()
            .CallAsyncMethod(objectWithMethod, methodNameToCall, methodArgumentsAsVariableNames);

        _body = _body.AddStatements(ExpressionStatement(statementBuilder.Build()));
        return this;
    }

    public MethodBodyBuilder CallGenericAsyncMethod(
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
    
    public MethodBodyBuilder Return()
    {
        _body = _body.AddStatements(ReturnStatement());
        return this;
    }

    public MethodBodyBuilder Return(Func<ExpressionBuilder, ExpressionBuilder> expressionBuilderFunc)
    {
        var statement = expressionBuilderFunc(new ExpressionBuilder()).Build();
        _body = _body.AddStatements(ReturnStatement(statement));
        return this;
    }
    
    public BlockSyntax Build()
    {
        return _body;
    }

    public MethodBodyBuilder AssignVariable(string assignTo, string from)
    {
        var assignmentExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(assignTo),
            IdentifierName(from)
        );
        _body = _body.AddStatements(ExpressionStatement(assignmentExpression));
        return this;
    }
    
    public MethodBodyBuilder IfNull(string variableName, Func<MethodBodyBuilder, MethodBodyBuilder> bodyBuilderFunc)
    {
        var ifBody = new MethodBodyBuilder();
        bodyBuilderFunc(ifBody);
        var ifStatement = IfStatement(ParseExpression($"{variableName} == null"), ifBody.Build());
        _body = _body.AddStatements(ifStatement);
        return this;
    }
    
    public MethodBodyBuilder ThrowEntityNotFoundException(string entityTypeName)
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
    
    public MethodBodyBuilder InitArrayVariable(string typeName, string variableName, IEnumerable<string> parameters)
    {
        var arrayType = ArrayType(ParseTypeName(typeName))
            .WithRankSpecifiers(
                SingletonList(
                    ArrayRankSpecifier(SeparatedList<ExpressionSyntax>())));


        var arrayVariableDeclaration = VariableDeclarator(Identifier(variableName))
            .WithInitializer(
                EqualsValueClause(
                    ArrayCreationExpression(arrayType)
                        .WithInitializer(InitializerExpression(
                            SyntaxKind.ArrayInitializerExpression,
                            SeparatedList<ExpressionSyntax>(
                                parameters.Select(IdentifierName).ToArray()
                            )
                        ))
                )
            );

        var variableDeclarationResultVariable = VariableDeclaration(ParseTypeName("var"))
            .WithVariables(SeparatedList<VariableDeclaratorSyntax>().Add(arrayVariableDeclaration));

        _body = _body.AddStatements(LocalDeclarationStatement(variableDeclarationResultVariable));
        return this;
    }

    public MethodBodyBuilder InitStringArray(string variableName, IEnumerable<string> parameters)
    {
        var arrayType = ArrayType(ParseTypeName("string"))
            .WithRankSpecifiers(
                SingletonList(
                    ArrayRankSpecifier(SeparatedList<ExpressionSyntax>())));


        var arrayVariableDeclaration = VariableDeclarator(Identifier(variableName))
            .WithInitializer(
                EqualsValueClause(
                    ArrayCreationExpression(arrayType)
                        .WithInitializer(InitializerExpression(
                            SyntaxKind.ArrayInitializerExpression,
                            SeparatedList<ExpressionSyntax>(
                                parameters.Select(p => LiteralExpression(
                                    SyntaxKind.StringLiteralExpression, Literal(p))).ToArray()
                            )
                        ))
                )
            );

        var variableDeclarationResultVariable = VariableDeclaration(ParseTypeName("var"))
            .WithVariables(SeparatedList<VariableDeclaratorSyntax>().Add(arrayVariableDeclaration));

        _body = _body.AddStatements(LocalDeclarationStatement(variableDeclarationResultVariable));
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