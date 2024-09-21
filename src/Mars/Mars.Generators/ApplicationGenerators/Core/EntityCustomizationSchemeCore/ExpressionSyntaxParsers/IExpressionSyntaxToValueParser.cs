using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators.ApplicationGenerators.Core.EntityCustomizationSchemeCore.ExpressionSyntaxParsers;

internal interface IExpressionSyntaxToValueParser
{
    bool CanParse(GeneratorExecutionContext context, ExpressionSyntax expression);

    object? Parse(GeneratorExecutionContext context, ExpressionSyntax expression);
}