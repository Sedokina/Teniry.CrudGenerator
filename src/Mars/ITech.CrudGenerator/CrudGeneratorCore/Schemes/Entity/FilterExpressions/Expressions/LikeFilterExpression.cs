using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;

internal class LikeFilterExpression : FilterExpression
{
    public LikeFilterExpression() : base(FilterType.Like)
    {
    }

    public override StatementSyntax BuildExpression(string filterPropertyName, string entityPropertyToFilter)
    {
        var likeArguments =
            SyntaxFactory.SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]
                {
                    SyntaxFactory.Argument(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind
                                .SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("x"),
                            SyntaxFactory.IdentifierName(
                                entityPropertyToFilter))),
                    SyntaxFactory.Token(SyntaxKind.CommaToken),
                    SyntaxFactory.Argument(
                        SyntaxFactory
                            .InterpolatedStringExpression(SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken))
                            .WithContents(
                                SyntaxFactory.List(
                                    new InterpolatedStringContentSyntax[]
                                    {
                                        SyntaxFactory.InterpolatedStringText()
                                            .WithTextToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(),
                                                SyntaxKind.InterpolatedStringTextToken,
                                                "%",
                                                "%",
                                                SyntaxFactory.TriviaList())),
                                        SyntaxFactory.Interpolation(SyntaxFactory.IdentifierName(filterPropertyName)),
                                        SyntaxFactory.InterpolatedStringText()
                                            .WithTextToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(),
                                                SyntaxKind.InterpolatedStringTextToken,
                                                "%",
                                                "%",
                                                SyntaxFactory.TriviaList()))
                                    })))
                });


        var result = SyntaxFactory.IfStatement(
            SyntaxFactory.IsPatternExpression(
                SyntaxFactory.IdentifierName(filterPropertyName),
                SyntaxFactory.UnaryPattern(
                    SyntaxFactory.ConstantPattern(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.NullLiteralExpression)))),
            SyntaxFactory.Block(
                SyntaxFactory.SingletonList<StatementSyntax>(
                    SyntaxFactory.ExpressionStatement(
                        SyntaxFactory.AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            SyntaxFactory.IdentifierName("query"),
                            SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("query"),
                                        SyntaxFactory.IdentifierName("Where")))
                                .WithArgumentList(
                                    SyntaxFactory.ArgumentList(
                                        SyntaxFactory.SingletonSeparatedList(
                                            SyntaxFactory.Argument(
                                                SyntaxFactory.SimpleLambdaExpression(
                                                        SyntaxFactory.Parameter(
                                                            SyntaxFactory.Identifier("x")))
                                                    .WithExpressionBody(
                                                        SyntaxFactory.InvocationExpression(
                                                                SyntaxFactory.MemberAccessExpression(
                                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                                    SyntaxFactory.MemberAccessExpression(
                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                        SyntaxFactory.IdentifierName("EF"),
                                                                        SyntaxFactory.IdentifierName("Functions")),
                                                                    SyntaxFactory.IdentifierName("ILike")))
                                                            .WithArgumentList(
                                                                SyntaxFactory.ArgumentList(likeArguments))))))))))));
        return result;
    }
}