using System;
using System.Linq;
using Mars.Generators.CrudGeneratorCore.ConfigurationsReceiver;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators.CrudGeneratorCore.Schemes.EntityCustomization.ExpressionSyntaxParsers;

internal class ObjectCreationToObjectParser : IExpressionSyntaxToValueParser
{
    private readonly PropertyAssignmentExpressionToPropertyNameAndValueParser _propertyAssignmentParser;

    public ObjectCreationToObjectParser(
        PropertyAssignmentExpressionToPropertyNameAndValueParser propertyAssignmentParser)
    {
        _propertyAssignmentParser = propertyAssignmentParser;
    }

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

        if (name != nameof(EntityGeneratorCreateOperationConfiguration))
        {
            return false;
        }


        return true;
    }

    public object? Parse(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        var objectCreationExpression = (ObjectCreationExpressionSyntax)expression;
        var result = new EntityCreateOperationCustomizationScheme();
        if (objectCreationExpression.Initializer == null)
        {
            return result;
        }

        var assignmentExpressions = objectCreationExpression.Initializer.Expressions
            .ToList()
            .OfType<AssignmentExpressionSyntax>()
            .ToList();

        var resultType = result.GetType();
        foreach (var assignmentExpression in assignmentExpressions)
        {
            if (!_propertyAssignmentParser.CanParse(context, assignmentExpression))
            {
                continue;
            }

            var (propertyName, value) = _propertyAssignmentParser
                .Parse(context, assignmentExpression) as Tuple<string, object?>;

            var property = resultType.GetProperty(propertyName);
            property?.SetValue(result, value);
        }

        return result;
    }
}