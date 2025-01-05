using System.Text;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;

internal class GreaterThanOrEqualFilterExpression : FilterExpression
{
    public GreaterThanOrEqualFilterExpression() : base(FilterType.GreaterThanOrEqual)
    {
    }

    public override StatementSyntax BuildExpression(string filterPropertyName, string entityPropertyToFilter)
    {
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
                                                        SyntaxFactory.BinaryExpression(
                                                            SyntaxKind.GreaterThanOrEqualExpression,
                                                            SyntaxFactory.MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                SyntaxFactory.IdentifierName("x"),
                                                                SyntaxFactory.IdentifierName(entityPropertyToFilter)),
                                                            SyntaxFactory.IdentifierName(filterPropertyName))))))))))));
        return result;
    }
}