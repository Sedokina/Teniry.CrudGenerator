using ITech.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Expressions;

internal class ContainsFilterExpression : FilterExpression
{
    public ContainsFilterExpression() : base(FilterType.Contains)
    {
    }

    public override StatementSyntax BuildExpression(string filterPropertyName, string entityPropertyToFilter)
    {
        var result = IfStatement(
            BinaryExpression(
                SyntaxKind.LogicalAndExpression,
                IsPatternExpression(
                    IdentifierName(filterPropertyName),
                    UnaryPattern(
                        ConstantPattern(
                            LiteralExpression(
                                SyntaxKind.NullLiteralExpression)))),
                BinaryExpression(
                    SyntaxKind.GreaterThanExpression,
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(filterPropertyName),
                        IdentifierName("Length")),
                    LiteralExpression(
                        SyntaxKind.NumericLiteralExpression,
                        Literal(0)))),
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
                                                                    IdentifierName(filterPropertyName),
                                                                    IdentifierName("Contains")))
                                                            .WithArgumentList(
                                                                ArgumentList(
                                                                    SingletonSeparatedList(
                                                                        Argument(
                                                                            MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                IdentifierName("x"),
                                                                                IdentifierName(
                                                                                    entityPropertyToFilter))))))))))))))));
        return result;
    }
}