using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators.ApplicationGenerators.Core.EntityCustomizationSchemeCore.ExpressionSyntaxParsers;

internal class LiteralExpressionSyntaxToValueParser : IExpressionSyntaxToValueParser
{
    public bool CanParse(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        return expression is LiteralExpressionSyntax;
    }

    public object Parse(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        var model = context.Compilation.GetSemanticModel(expression.SyntaxTree);
        var constant = model.GetConstantValue(expression);
        return constant.Value;
    }
}