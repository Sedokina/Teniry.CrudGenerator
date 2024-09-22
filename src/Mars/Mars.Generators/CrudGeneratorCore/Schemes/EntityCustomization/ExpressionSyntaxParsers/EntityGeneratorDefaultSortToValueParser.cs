using Mars.Generators.CrudGeneratorCore.Schemes.Entity;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators.CrudGeneratorCore.Schemes.EntityCustomization.ExpressionSyntaxParsers;

internal class EntityGeneratorDefaultSortToValueParser : IExpressionSyntaxToValueParser
{
    private readonly LiteralExpressionSyntaxToValueParser _literalExpressionSyntaxToValueParser;

    public EntityGeneratorDefaultSortToValueParser(
        LiteralExpressionSyntaxToValueParser literalExpressionSyntaxToValueParser)
    {
        _literalExpressionSyntaxToValueParser = literalExpressionSyntaxToValueParser;
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
        if (name != "EntityGeneratorDefaultSort")
        {
            return false;
        }


        return true;
    }

    public object Parse(GeneratorExecutionContext context, ExpressionSyntax expression)
    {
        var objectCreationExpression = (ObjectCreationExpressionSyntax)expression;
        if (objectCreationExpression.ArgumentList is null ||
            objectCreationExpression.ArgumentList.Arguments.Count != 2)
        {
            return false;
        }

        var arguments = objectCreationExpression.ArgumentList.Arguments;
        if (arguments[0].Expression is not LiteralExpressionSyntax literalExpressionSyntax ||
            arguments[1].Expression is not SimpleLambdaExpressionSyntax lambdaExpressionSyntax ||
            lambdaExpressionSyntax.ExpressionBody is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            return false;
        }

        var direction = _literalExpressionSyntaxToValueParser.Parse(context, literalExpressionSyntax);
        var fieldName = memberAccessExpressionSyntax.Name.ToString();
        return new EntityDefaultSort(direction!.ToString().Equals("asc") ? "asc" : "desc", fieldName);
    }
}