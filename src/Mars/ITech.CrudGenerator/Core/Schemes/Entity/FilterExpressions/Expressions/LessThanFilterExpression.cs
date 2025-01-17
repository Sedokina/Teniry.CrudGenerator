using ITech.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Core;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ITech.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Expressions;

internal class LessThanFilterExpression : FilterExpression {
    public LessThanFilterExpression() : base(FilterType.LessThan) { }

    public override StatementSyntax BuildExpression(string filterPropertyName, string entityPropertyToFilter) {
        var result = IfStatement(
            IsPatternExpression(
                IdentifierName(filterPropertyName),
                UnaryPattern(ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression)))
            ),
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
                                        IdentifierName("Where")
                                    )
                                )
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList(
                                            Argument(
                                                SimpleLambdaExpression(Parameter(Identifier("x")))
                                                    .WithExpressionBody(
                                                        BinaryExpression(
                                                            SyntaxKind.LessThanExpression,
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName("x"),
                                                                IdentifierName(entityPropertyToFilter)
                                                            ),
                                                            IdentifierName(filterPropertyName)
                                                        )
                                                    )
                                            )
                                        )
                                    )
                                )
                        )
                    )
                )
            )
        );

        return result;
    }
}