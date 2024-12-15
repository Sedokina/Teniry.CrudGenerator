using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.EntityCustomization.ExpressionSyntaxParsers;

internal class LiteralExpressionToValueParser : IExpressionSyntaxToValueParser
{
    public bool CanParse(Compilation compilation, ExpressionSyntax expression)
    {
        return expression is LiteralExpressionSyntax;
    }

    public object? Parse(Compilation compilation, ExpressionSyntax expression)
    {
        var model = compilation.GetSemanticModel(expression.SyntaxTree);
        var constant = model.GetConstantValue(expression);
        return constant.Value;
    }
}