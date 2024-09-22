using System;
using System.Linq;
using Mars.Generators.CrudGeneratorCore.ConfigurationsReceiver;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators.CrudGeneratorCore.Schemes.EntityCustomization.ExpressionSyntaxParsers;

public class ObjectCreationToObjectParser : IExpressionSyntaxToValueParser
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

        if (name != nameof(EntityGeneratorCreateOperationConfiguration))
        {
            return false;
        }


        return true;
    }

    public object? Parse(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        var objectCreationExpression = (ObjectCreationExpressionSyntax)expression;
        var result = new EntityCustomizationCreateOperationScheme();
        if (objectCreationExpression.Initializer == null)
        {
            return result;
        }

        var assignmentExpressions = objectCreationExpression.Initializer.Expressions
            .ToList()
            .OfType<AssignmentExpressionSyntax>()
            .ToList();

        var assignmentExpressionParer = new PropertyAssignmentExpressionToPropertyNameAndValueParser();
        var resultType = result.GetType();
        foreach (var assignmentExpression in assignmentExpressions)
        {
            if (!assignmentExpressionParer.CanParse(context, assignmentExpression))
            {
                continue;
            }

            var (propertyName, value) =
                assignmentExpressionParer.Parse(context, assignmentExpression) as Tuple<string, object?>;

            var property = resultType.GetProperty(propertyName);
            property?.SetValue(result, value);
        }

        return result;
    }
}