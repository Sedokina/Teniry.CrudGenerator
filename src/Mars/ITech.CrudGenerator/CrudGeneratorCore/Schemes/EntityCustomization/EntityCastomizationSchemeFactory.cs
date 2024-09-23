using System;
using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.Abstractions;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.EntityCustomization.ExpressionSyntaxParsers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.EntityCustomization;

internal class EntityCastomizationSchemeFactory
{
    internal static EntityCustomizationScheme Construct(
        INamedTypeSymbol? generatorSymbol,
        GeneratorExecutionContext context)
    {
        var generatorScheme = new EntityCustomizationScheme();
        if (!TryExtractValidConstructorDeclaration(generatorSymbol, out var generatorConstructorDeclaration))
        {
            return generatorScheme;
        }

        if (!TryGetConstructorStatements(generatorConstructorDeclaration, out var constructorStatements))
        {
            return generatorScheme;
        }


        var assignmentExpressionParer = ConstructAvailableParsers();

        var generatorSchemeType = generatorScheme.GetType();
        foreach (var statementSyntax in constructorStatements)
        {
            if (!assignmentExpressionParer.CanParse(context, statementSyntax.Expression))
            {
                continue;
            }

            var (propertyName, value) = assignmentExpressionParer
                .Parse(context, statementSyntax.Expression) as Tuple<string, object?>;

            var property = generatorSchemeType.GetProperty(propertyName);
            property?.SetValue(generatorScheme, value);
        }

        return generatorScheme;
    }

    private static PropertyAssignmentExpressionToPropertyNameAndValueParser ConstructAvailableParsers()
    {
        List<IExpressionSyntaxToValueParser> availableAssignmentExpressionsRightSideParsers =
        [
            new LiteralExpressionToValueParser(),
            new EntityGeneratorDefaultSortToValueParser(new LiteralExpressionToValueParser()),
        ];

        var assignmentExpressionParer = new PropertyAssignmentExpressionToPropertyNameAndValueParser(
            availableAssignmentExpressionsRightSideParsers);

        // These parsers are added in the end because they depend on assignment parser
        // but assignment parsed depends on the list, where these parsers should be included
        availableAssignmentExpressionsRightSideParsers
            .Add(new ObjectCreationToObjectParser<
                EntityGeneratorCreateOperationConfiguration,
                EntityCreateOperationCustomizationScheme>(assignmentExpressionParer));
        availableAssignmentExpressionsRightSideParsers
            .Add(new ObjectCreationToObjectParser<
                EntityGeneratorDeleteOperationConfiguration,
                EntityDeleteOperationCustomizationScheme>(assignmentExpressionParer));
        availableAssignmentExpressionsRightSideParsers
            .Add(new ObjectCreationToObjectParser<
                EntityGeneratorUpdateOperationConfiguration,
                EntityUpdateOperationCustomizationScheme>(assignmentExpressionParer));
        availableAssignmentExpressionsRightSideParsers
            .Add(new ObjectCreationToObjectParser<
                EntityGeneratorGetByIdOperationConfiguration,
                EntityGetByIdOperationCustomizationScheme>(assignmentExpressionParer));
        availableAssignmentExpressionsRightSideParsers
            .Add(new ObjectCreationToObjectParser<
                EntityGeneratorGetListOperationConfiguration,
                EntityGetListOperationCustomizationScheme>(assignmentExpressionParer));
        return assignmentExpressionParer;
    }

    private static bool TryGetConstructorStatements(
        ConstructorDeclarationSyntax? generatorConstructorDeclaration,
        out List<ExpressionStatementSyntax> constructorStatements)
    {
        constructorStatements = [];
        if (generatorConstructorDeclaration?.Body is null || generatorConstructorDeclaration.Body.Statements.Count == 0)
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
    /// <param name="constructorDeclarationSyntax"></param>
    /// <returns>Parameterless constructor declaration of <see cref="EntityGeneratorConfiguration{T}"/></returns>
    /// <exception cref="Exception">
    ///     When: <br/>
    ///     - <see cref="generatorSymbol"/> is null or not INamedTypeSymbol <br/>
    ///     - constructor of entity generator is not parameterless <br/>
    ///     - failed to get constructor of <see cref="EntityGeneratorConfiguration{T}"/>
    ///         as <see cref="ConstructorDeclarationSyntax"/> <br/>
    /// </exception>
    private static bool TryExtractValidConstructorDeclaration(
        INamedTypeSymbol? generatorSymbol,
        out ConstructorDeclarationSyntax? constructorDeclarationSyntax)
    {
        if (generatorSymbol is null)
        {
            throw new Exception("Failed to read one of declared Entity Generator Configuration");
        }

        constructorDeclarationSyntax = null;
        // Get first parameterless constructor
        var generatorConstructorMethodSymbol = generatorSymbol
            .Constructors
            .FirstOrDefault(x => x.Parameters.Length == 0);
        if (generatorConstructorMethodSymbol == null)
        {
            return false;
        }

        var generatorConstructorDeclaration = generatorConstructorMethodSymbol.DeclaringSyntaxReferences
            .FirstOrDefault(x => x.GetSyntax() is ConstructorDeclarationSyntax)?
            .GetSyntax() as ConstructorDeclarationSyntax;

        // When this is default parameterless constructor, not defined by user,
        // declaration is null
        if (generatorConstructorDeclaration == null)
        {
            return false;
        }

        constructorDeclarationSyntax = generatorConstructorDeclaration;
        return true;
    }
}