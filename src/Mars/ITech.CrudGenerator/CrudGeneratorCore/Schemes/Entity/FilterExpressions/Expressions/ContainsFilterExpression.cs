using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;

internal class ContainsFilterExpression : FilterExpression
{
    public ContainsFilterExpression() : base(FilterType.Contains)
    {
    }

    public override StatementSyntax BuildExpression(string filterPropertyName, string entityPropertyToFilter)
    {
        
        var result = SyntaxFactory.IfStatement(
            SyntaxFactory.BinaryExpression(
                SyntaxKind.LogicalAndExpression,
                SyntaxFactory.IsPatternExpression(
                    SyntaxFactory.IdentifierName(filterPropertyName),
                    SyntaxFactory.UnaryPattern(
                        SyntaxFactory.ConstantPattern(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.NullLiteralExpression)))),
                SyntaxFactory.BinaryExpression(
                    SyntaxKind.GreaterThanExpression,
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(filterPropertyName),
                        SyntaxFactory.IdentifierName("Length")),
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.NumericLiteralExpression,
                        SyntaxFactory.Literal(0)))),
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
                                                                    SyntaxFactory.IdentifierName(filterPropertyName),
                                                                    SyntaxFactory.IdentifierName("Contains")))
                                                            .WithArgumentList(
                                                                SyntaxFactory.ArgumentList(
                                                                    SyntaxFactory.SingletonSeparatedList(
                                                                        SyntaxFactory.Argument(
                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                SyntaxFactory.IdentifierName("x"),
                                                                                SyntaxFactory
                                                                                    .IdentifierName(
                                                                                        entityPropertyToFilter))))))))))))))));
        return result;
    }
}