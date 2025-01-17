using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.Core.Schemes.InternalEntityGenerator.ExpressionSyntaxParsers;

internal class PropertyAssignmentExpressionToPropertyNameAndValueParser : IExpressionSyntaxToValueParser {
    private readonly List<IExpressionSyntaxToValueParser> _availableRightSideParsers;

    public PropertyAssignmentExpressionToPropertyNameAndValueParser(
        List<IExpressionSyntaxToValueParser> availableRightSideParsers
    ) {
        _availableRightSideParsers = availableRightSideParsers;
    }

    public bool CanParse(Compilation compilation, ExpressionSyntax expression) {
        if (expression is not AssignmentExpressionSyntax assignmentExpression) {
            return false;
        }

        return CanParseLeftSide(compilation, assignmentExpression.Left) &&
            CanParseRightSide(compilation, assignmentExpression.Right);
    }

    public object? Parse(Compilation compilation, ExpressionSyntax expression) {
        var assignmentExpression = (AssignmentExpressionSyntax)expression;
        var propertyName = ParseLeftSide(compilation, assignmentExpression.Left);
        var value = ParseRightSide(compilation, assignmentExpression.Right);

        return new Tuple<string, object?>(propertyName, value);
    }

    private object? ParseRightSide(Compilation compilation, ExpressionSyntax expression) {
        foreach (var expressionSyntaxParser in _availableRightSideParsers) {
            if (!expressionSyntaxParser.CanParse(compilation, expression)) continue;

            return expressionSyntaxParser.Parse(compilation, expression);
        }

        return null;
    }

    private bool CanParseLeftSide(Compilation compilation, ExpressionSyntax expressionLeftSide) {
        var model = compilation.GetSemanticModel(expressionLeftSide.SyntaxTree);
        var symbolInfo = model.GetSymbolInfo(expressionLeftSide);

        return symbolInfo.Symbol is IPropertySymbol;
    }

    private bool CanParseRightSide(Compilation compilation, ExpressionSyntax expressionRightSide) {
        return _availableRightSideParsers.Any(x => x.CanParse(compilation, expressionRightSide));
    }

    private string ParseLeftSide(Compilation compilation, ExpressionSyntax expression) {
        var model = compilation.GetSemanticModel(expression.SyntaxTree);
        var symbolInfo = model.GetSymbolInfo(expression);
        var propertySymbol = (IPropertySymbol)symbolInfo.Symbol!;

        return propertySymbol.Name;
    }
}