using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators.CrudGeneratorCore.Schemes.EntityCustomization.ExpressionSyntaxParsers;

internal class PropertyAssignmentExpressionToPropertyNameAndValueParser : IExpressionSyntaxToValueParser
{
    private readonly List<IExpressionSyntaxToValueParser> _availableRightSideParsers;

    public PropertyAssignmentExpressionToPropertyNameAndValueParser(
        List<IExpressionSyntaxToValueParser> availableRightSideParsers)
    {
        _availableRightSideParsers = availableRightSideParsers;
    }

    public bool CanParse(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        if (expression is not AssignmentExpressionSyntax assignmentExpression)
        {
            return false;
        }

        return CanParseLeftSide(context, assignmentExpression.Left) &&
               CanParseRightSide(context, assignmentExpression.Right);
    }

    public object? Parse(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        var assignmentExpression = (AssignmentExpressionSyntax)expression;
        var propertyName = ParseLeftSide(context, assignmentExpression.Left);
        var value = ParseRightSide(context, assignmentExpression.Right);

        return new Tuple<string, object?>(propertyName, value);
    }

    private object? ParseRightSide(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        foreach (var expressionSyntaxParser in _availableRightSideParsers)
        {
            if (!expressionSyntaxParser.CanParse(context, expression)) continue;
            return expressionSyntaxParser.Parse(context, expression);
        }

        return null;
    }

    private bool CanParseLeftSide(GeneratorExecutionContext context, ExpressionSyntax expressionLeftSide)
    {
        var model = context.Compilation.GetSemanticModel(expressionLeftSide.SyntaxTree);
        var symbolInfo = model.GetSymbolInfo(expressionLeftSide);
        return symbolInfo.Symbol is IPropertySymbol;
    }

    private bool CanParseRightSide(GeneratorExecutionContext context, ExpressionSyntax expressionRightSide)
    {
        return _availableRightSideParsers.Any(x => x.CanParse(context, expressionRightSide));
    }

    private string ParseLeftSide(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        var model = context.Compilation.GetSemanticModel(expression.SyntaxTree);
        var symbolInfo = model.GetSymbolInfo(expression);
        var propertySymbol = (IPropertySymbol)symbolInfo.Symbol!;
        return propertySymbol.Name;
    }
}