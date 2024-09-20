using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators.ApplicationGenerators.Core.EntityCustomizationSchemeCore;

internal class EntityCastomizationSchemeFactory
{
    internal static EntityCustomizationScheme Constrcut(
        INamedTypeSymbol? generatorSymbol,
        GeneratorExecutionContext context)
    {
        var generatorConstructorDeclaration = ExtractValidConstrcutorDeclaration(generatorSymbol);
        var generatorScheme = new EntityCustomizationScheme();
        if (!TryGetConstructorStatements(generatorConstructorDeclaration, out var constructorStatements))
        {
            return generatorScheme;
        }

        var generatorSchemeType = generatorScheme.GetType();
        foreach (var statementSyntax in constructorStatements)
        {
            if (!TryParseConstructorStatement(context, statementSyntax, out var propertyName, out var value))
            {
                continue;
            }

            var property = generatorSchemeType.GetProperty(propertyName);
            property?.SetValue(generatorScheme, value);
        }

        return generatorScheme;
    }

    private static bool TryParseConstructorStatement(
        GeneratorExecutionContext context,
        ExpressionStatementSyntax statementSyntax,
        out string propertyName,
        out object value)
    {
        propertyName = "";
        value = null;
        if (statementSyntax.Expression is not AssignmentExpressionSyntax assignmentExpressionSyntax)
        {
            return false;
        }

        if (!TryParseConstructorStatementLeftSide(context, assignmentExpressionSyntax.Left, ref propertyName))
        {
            return false;
        }

        if (!TryParseExpressionRightSide(context, assignmentExpressionSyntax.Right, ref value))
        {
            return false;
        }

        return true;
    }

    private static bool TryParseConstructorStatementLeftSide(
        GeneratorExecutionContext context,
        ExpressionSyntax expressionLeftSide,
        ref string propertyName)
    {
        var model = context.Compilation.GetSemanticModel(expressionLeftSide.SyntaxTree);
        var symbolInfo = model.GetSymbolInfo(expressionLeftSide);
        if (symbolInfo.Symbol is not IPropertySymbol propertySymbol)
        {
            return false;
        }

        propertyName = propertySymbol.Name;
        return true;
    }

    private static bool TryParseExpressionRightSide(
        GeneratorExecutionContext context,
        ExpressionSyntax expressionRightSide,
        ref object value)
    {
        if (expressionRightSide is LiteralExpressionSyntax)
        {
            value = GetSyntaxNodeAsLiteral(context, expressionRightSide);
            return true;
        }

        if (expressionRightSide is ObjectCreationExpressionSyntax objectCreationExpression)
        {
            var model = context.Compilation.GetSemanticModel(expressionRightSide.SyntaxTree);
            var symbolInfo = model.GetSymbolInfo(expressionRightSide);
            if (symbolInfo.Symbol is not IMethodSymbol constructorSymbol)
            {
                return false;
            }

            var name = constructorSymbol.ContainingSymbol.Name;
            if (name != "EntityGeneratorDefaultSort")
            {
                return false;
            }

            if (objectCreationExpression.ArgumentList is null ||
                objectCreationExpression.ArgumentList.Arguments.Count != 2)
            {
                return false;
            }

            if (objectCreationExpression.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax
                    literalExpressionSyntax &&
                objectCreationExpression.ArgumentList.Arguments[1].Expression is SimpleLambdaExpressionSyntax
                    lambdaExpressionSyntax &&
                lambdaExpressionSyntax.ExpressionBody is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                var direction = GetSyntaxNodeAsLiteral(context, literalExpressionSyntax);
                var fieldName = memberAccessExpressionSyntax.Name.ToString();
                value = new EntityCustomizationSchemeDefaultSort(direction.ToString(), fieldName);
                return true;
            }
        }

        return false;
    }

    private static object GetSyntaxNodeAsLiteral(
        GeneratorExecutionContext context,
        SyntaxNode expressionRightSide)
    {
        var model = context.Compilation.GetSemanticModel(expressionRightSide.SyntaxTree);
        var constant = model.GetConstantValue(expressionRightSide);
        return constant.Value;
    }


    private static bool TryGetConstructorStatements(
        ConstructorDeclarationSyntax generatorConstructorDeclaration,
        out List<ExpressionStatementSyntax> constructorStatements)
    {
        constructorStatements = [];
        if (generatorConstructorDeclaration.Body is null || generatorConstructorDeclaration.Body.Statements.Count == 0)
        {
            return false;
        }

        constructorStatements = generatorConstructorDeclaration.Body.Statements
            .OfType<ExpressionStatementSyntax>()
            .ToList();

        if (constructorStatements.Count == 0)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Get constructor of <see cref="EntityGeneratorConfiguration{TEntity}"/> by it's symbol
    ///     received from class defined in client's assembly
    /// </summary>
    /// <param name="generatorSymbol">
    ///     <see cref="EntityGeneratorConfiguration{T}"/>'s Symbol received from class defined in client's assembly
    /// </param>
    /// <returns>Parameterless constructor declaration of <see cref="EntityGeneratorConfiguration{T}"/></returns>
    /// <exception cref="Exception">
    ///     When: <br/>
    ///     - <see cref="generatorSymbol"/> is null or not INamedTypeSymbol <br/>
    ///     - constructor of entity generator is not parameterless <br/>
    ///     - failed to get constructor of <see cref="EntityGeneratorConfiguration{T}"/>
    ///         as <see cref="ConstructorDeclarationSyntax"/> <br/>
    /// </exception>
    private static ConstructorDeclarationSyntax ExtractValidConstrcutorDeclaration(INamedTypeSymbol generatorSymbol)
    {
        if (generatorSymbol is null)
        {
            throw new Exception("Failed to read one of declared Entity Generator Configuration");
        }

        // Get first parameterless constructor
        var generatorConstructorMethodSymbol = generatorSymbol
            .Constructors
            .FirstOrDefault(x => x.Parameters.Length == 0);
        if (generatorConstructorMethodSymbol == null)
        {
            throw new Exception($"Constructor of {generatorSymbol.Name} should be parameterless");
        }

        var generatorConstructorDeclaration = generatorConstructorMethodSymbol.DeclaringSyntaxReferences
            .First(x => x.GetSyntax() is ConstructorDeclarationSyntax)
            .GetSyntax() as ConstructorDeclarationSyntax;

        if (generatorConstructorDeclaration is null)
        {
            throw new Exception(
                $"Failed to read constructor of {generatorSymbol.Name} as {nameof(ConstructorDeclarationSyntax)}");
        }

        return generatorConstructorDeclaration;
    }
}