using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator.ExpressionSyntaxParsers;

internal interface IExpressionSyntaxToValueParser {
    bool CanParse(Compilation compilation, ExpressionSyntax expression);

    object? Parse(Compilation compilation, ExpressionSyntax expression);
}