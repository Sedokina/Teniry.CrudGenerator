using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;

internal class LikeMongoFilterExpression : FilterExpression
{
    public LikeMongoFilterExpression() : base(FilterType.Like)
    {
    }

    public override StatementSyntax BuildExpression(string filterPropertyName, string entityPropertyToFilter)
    {
        var whereArguments = ArgumentList(
            SingletonSeparatedList(
                Argument(
                    SimpleLambdaExpression(
                            Parameter(
                                Identifier("x")))
                        .WithExpressionBody(
                            InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("x"),
                                                    IdentifierName(entityPropertyToFilter)),
                                                IdentifierName("ToLower"))),
                                        IdentifierName("Contains")))
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName(filterPropertyName),
                                                        IdentifierName("ToLower")))))))))));
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
                                .WithArgumentList(whereArguments))))));

        return result;
    }
}