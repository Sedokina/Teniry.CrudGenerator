using ITech.CrudGenerator.CrudGeneratorCore.Configurations.GeneratorConfigurations.TypedConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator.ExpressionSyntaxParsers;

internal class EntityGeneratorDefaultSortToValueParser : IExpressionSyntaxToValueParser
{
    private readonly LiteralExpressionToValueParser _literalExpressionToValueParser;

    public EntityGeneratorDefaultSortToValueParser(
        LiteralExpressionToValueParser literalExpressionToValueParser)
    {
        _literalExpressionToValueParser = literalExpressionToValueParser;
    }

    public bool CanParse(Compilation compilation, ExpressionSyntax expression)
    {
        if (expression is not ObjectCreationExpressionSyntax)
        {
            return false;
        }

        var model = compilation.GetSemanticModel(expression.SyntaxTree);
        var symbolInfo = model.GetSymbolInfo(expression);
        if (symbolInfo.Symbol is not IMethodSymbol constructorSymbol)
        {
            return false;
        }

        var name = constructorSymbol.ContainingSymbol.Name;
        if (name != TypeNamesForAnalyzers.EntityGeneratorDefaultSort)
        {
            return false;
        }


        return true;
    }

    public object Parse(Compilation compilation, ExpressionSyntax expression)
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

        var direction = _literalExpressionToValueParser.Parse(compilation, literalExpressionSyntax);
        var fieldName = memberAccessExpressionSyntax.Name.ToString();
        return new EntityDefaultSort(direction!.ToString().Equals("asc") ? "asc" : "desc", fieldName);
    }
}