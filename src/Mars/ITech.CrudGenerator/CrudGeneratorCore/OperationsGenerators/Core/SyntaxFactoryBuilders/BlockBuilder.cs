using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core.SyntaxFactoryBuilders;

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

    public BlockBuilder CallMethod(string objectWithMethod, string methodNameToCall, List<ExpressionSyntax> arguments)
    {
        var statementBuilder =
            SimpleSyntaxFactory.CallMethod(objectWithMethod, methodNameToCall, arguments);

        _body = _body.AddStatements(ExpressionStatement(statementBuilder));
        return this;
    }
    
    public BlockBuilder CallAsyncMethod(string objectWithMethod, string methodNameToCall, List<ExpressionSyntax> arguments)
    {
        var statementBuilder = SimpleSyntaxFactory
            .CallAsyncMethod(objectWithMethod, methodNameToCall, arguments);

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