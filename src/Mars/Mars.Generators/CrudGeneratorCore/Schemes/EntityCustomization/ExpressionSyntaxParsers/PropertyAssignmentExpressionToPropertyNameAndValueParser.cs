using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators.CrudGeneratorCore.Schemes.EntityCustomization.ExpressionSyntaxParsers;

public class PropertyAssignmentExpressionToPropertyNameAndValueParser : IExpressionSyntaxToValueParser
{
    private static readonly List<IExpressionSyntaxToValueParser> ExpressionSyntaxParsers =
    [
        new LiteralExpressionToValueParser(),
        new EntityGeneratorDefaultSortToValueParser(new LiteralExpressionToValueParser()),
        new ObjectCreationToObjectParser()
    ];

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

    private static object? ParseRightSide(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        foreach (var expressionSyntaxParser in ExpressionSyntaxParsers)
        {
            if (!expressionSyntaxParser.CanParse(context, expression)) continue;
            return expressionSyntaxParser.Parse(context, expression);
        }

        return null;
    }

    private static bool CanParseLeftSide(
        GeneratorExecutionContext context,
        ExpressionSyntax expressionLeftSide)
    {
        var model = context.Compilation.GetSemanticModel(expressionLeftSide.SyntaxTree);
        var symbolInfo = model.GetSymbolInfo(expressionLeftSide);
        return symbolInfo.Symbol is IPropertySymbol;
    }

    private static bool CanParseRightSide(
        GeneratorExecutionContext context,
        ExpressionSyntax expressionRightSide)
    {
        return ExpressionSyntaxParsers.Any(x => x.CanParse(context, expressionRightSide));
    }

    private static string ParseLeftSide(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        var model = context.Compilation.GetSemanticModel(expression.SyntaxTree);
        var symbolInfo = model.GetSymbolInfo(expression);
        var propertySymbol = (IPropertySymbol)symbolInfo.Symbol!;
        return propertySymbol.Name;
    }
}