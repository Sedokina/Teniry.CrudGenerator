using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;

internal class LikeFilterExpression : FilterExpression
{
    public LikeFilterExpression() : base(FilterType.Like)
    {
    }

    public override StatementSyntax BuildExpression(string filterPropertyName, string entityPropertyToFilter)
    {
        var likeArguments =
            SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]
                {
                    Argument(
                        MemberAccessExpression(
                            SyntaxKind
                                .SimpleMemberAccessExpression,
                            IdentifierName("x"),
                            IdentifierName(
                                entityPropertyToFilter))),
                    Token(SyntaxKind.CommaToken),
                    Argument(
                        InterpolatedStringExpression(Token(SyntaxKind.InterpolatedStringStartToken))
                            .WithContents(
                                List(
                                    new InterpolatedStringContentSyntax[]
                                    {
                                        InterpolatedStringText()
                                            .WithTextToken(Token(TriviaList(),
                                                SyntaxKind.InterpolatedStringTextToken,
                                                "%",
                                                "%",
                                                TriviaList())),
                                        Interpolation(IdentifierName(filterPropertyName)),
                                        InterpolatedStringText()
                                            .WithTextToken(Token(TriviaList(),
                                                SyntaxKind.InterpolatedStringTextToken,
                                                "%",
                                                "%",
                                                TriviaList()))
                                    })))
                });


        var result = IfStatement(
            IsPatternExpression(
                IdentifierName(filterPropertyName),
                UnaryPattern(
                    ConstantPattern(
                        LiteralExpression(
                            SyntaxKind.NullLiteralExpression)))),
            Block(
                SingletonList<StatementSyntax>(
                    ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            IdentifierName("query"),
                            InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("query"),
                                        IdentifierName("Where")))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(
                                                SimpleLambdaExpression(
                                                        Parameter(
                                                            Identifier("x")))
                                                    .WithExpressionBody(
                                                        InvocationExpression(
                                                                MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        IdentifierName("EF"),
                                                                        IdentifierName("Functions")),
                                                                    IdentifierName("ILike")))
                                                            .WithArgumentList(
                                                                ArgumentList(likeArguments))))))))))));
        return result;
    }
}