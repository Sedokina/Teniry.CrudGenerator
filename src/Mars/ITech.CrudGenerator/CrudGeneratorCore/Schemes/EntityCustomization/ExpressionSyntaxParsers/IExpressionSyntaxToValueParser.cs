using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.EntityCustomization.ExpressionSyntaxParsers;

internal interface IExpressionSyntaxToValueParser
{
    bool CanParse(GeneratorExecutionContext context, ExpressionSyntax expression);

    object? Parse(GeneratorExecutionContext context, ExpressionSyntax expression);
}