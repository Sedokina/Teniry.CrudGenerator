using System.Linq;
using Mars.Generators.CrudGeneratorCore.ConfigurationsReceiver;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators.CrudGeneratorCore.Schemes.EntityCustomization.ExpressionSyntaxParsers;

public class OperationWithReturnValueCustomizationToValueParser : IExpressionSyntaxToValueParser
{
    public bool CanParse(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        if (expression is not ObjectCreationExpressionSyntax)
        {
            return false;
        }

        var model = context.Compilation.GetSemanticModel(expression.SyntaxTree);
        var symbolInfo = model.GetSymbolInfo(expression);
        if (symbolInfo.Symbol is not IMethodSymbol constructorSymbol)
        {
            return false;
        }

        var name = constructorSymbol.ContainingSymbol.Name;

        if (name != nameof(OperationWithReturnValueCustomization))
        {
            return false;
        }


        return true;
    }

    public object? Parse(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        var objectCreationExpression = expression as ObjectCreationExpressionSyntax;
        var expressions = objectCreationExpression.Initializer.Expressions
            .ToList()
            .OfType<AssignmentExpressionSyntax>()
            .ToList();

        var parser = new LiteralExpressionSyntaxToValueParser();
        foreach (var expressionSyntax in expressions)
        {
            
        }
        return null;
        // throw new System.NotImplementedException();
    }
}