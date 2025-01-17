using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.Core.Schemes.InternalEntityGenerator.ExpressionSyntaxParsers;

internal class ObjectCreationToObjectParser<TFrom, TTo> : IExpressionSyntaxToValueParser
    where TFrom : class
    where TTo : class, new() {
    private readonly PropertyAssignmentExpressionToPropertyNameAndValueParser _propertyAssignmentParser;

    public ObjectCreationToObjectParser(
        PropertyAssignmentExpressionToPropertyNameAndValueParser propertyAssignmentParser
    ) {
        _propertyAssignmentParser = propertyAssignmentParser;
    }

    public bool CanParse(Compilation compilation, ExpressionSyntax expression) {
        if (expression is not ObjectCreationExpressionSyntax &&
            expression is not ImplicitObjectCreationExpressionSyntax) {
            return false;
        }

        var model = compilation.GetSemanticModel(expression.SyntaxTree);
        var symbolInfo = model.GetSymbolInfo(expression);
        if (symbolInfo.Symbol is not IMethodSymbol constructorSymbol) {
            return false;
        }

        var name = constructorSymbol.ContainingSymbol.Name;

        if (name != typeof(TFrom).Name) {
            return false;
        }

        return true;
    }

    public object Parse(Compilation compilation, ExpressionSyntax expression) {
        var objectCreationExpression = (BaseObjectCreationExpressionSyntax)expression;
        var result = new TTo();
        if (objectCreationExpression.Initializer == null) {
            return result;
        }

        var assignmentExpressions = objectCreationExpression.Initializer.Expressions
            .ToList()
            .OfType<AssignmentExpressionSyntax>()
            .ToList();

        var resultType = result.GetType();
        foreach (var assignmentExpression in assignmentExpressions) {
            if (!_propertyAssignmentParser.CanParse(compilation, assignmentExpression)) {
                continue;
            }

            var (propertyName, value) = _propertyAssignmentParser
                .Parse(compilation, assignmentExpression) as Tuple<string, object?>;

            var property = resultType.GetProperty(propertyName);
            property?.SetValue(result, value);
        }

        return result;
    }
}