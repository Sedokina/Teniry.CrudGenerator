using System;
using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.Abstractions.Configuration;
using ITech.CrudGenerator.Core.Schemes.Entity.Extensions;
using ITech.CrudGenerator.Core.Schemes.InternalEntityGenerator.ExpressionSyntaxParsers;
using ITech.CrudGenerator.Core.Schemes.InternalEntityGenerator.Operations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.Core.Schemes.InternalEntityGenerator;

internal class InternalEntityGeneratorConfigurationFactory
{
    internal static InternalEntityGeneratorConfiguration Construct(
        INamedTypeSymbol generatorSymbol,
        Compilation compilation)
    {
        var generatorScheme = new InternalEntityGeneratorConfiguration(GetEntityClassMetadata(generatorSymbol));
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
            if (!assignmentExpressionParer.CanParse(compilation, statementSyntax.Expression))
            {
                continue;
            }

            var (propertyName, value) = assignmentExpressionParer
                .Parse(compilation, statementSyntax.Expression) as Tuple<string, object?>;

            var property = generatorSchemeType.GetProperty(propertyName);
            property?.SetValue(generatorScheme, value);
        }

        return generatorScheme;
    }

    private static InternalEntityClassMetadata GetEntityClassMetadata(INamedTypeSymbol? generatorSymbol)
    {
        var entityClassTypeSymbol = generatorSymbol?.BaseType?.TypeArguments.FirstOrDefault();
        if (entityClassTypeSymbol == null) return new InternalEntityClassMetadata("", "", "", []);
        var properties = entityClassTypeSymbol.OriginalDefinition.GetMembers().OfType<IPropertySymbol>()
            .Select(x => new InternalEntityClassPropertyMetadata(
                x.Name,
                x.Type.ToString(),
                x.Type.MetadataName,
                x.Type.SpecialType,
                x.Type.IsSimple(),
                x.Type.NullableAnnotation == NullableAnnotation.Annotated)
            );
        var internalEntityClassMetadata = new InternalEntityClassMetadata(
            entityClassTypeSymbol.Name,
            entityClassTypeSymbol.ContainingNamespace.ToString(),
            entityClassTypeSymbol.ContainingAssembly.Name,
            new EquatableList<InternalEntityClassPropertyMetadata>(properties)
        );

        return internalEntityClassMetadata;
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
                InternalEntityGeneratorCreateOperationConfiguration>(assignmentExpressionParer));
        availableAssignmentExpressionsRightSideParsers
            .Add(new ObjectCreationToObjectParser<
                EntityGeneratorDeleteOperationConfiguration,
                InternalEntityGeneratorDeleteOperationConfiguration>(assignmentExpressionParer));
        availableAssignmentExpressionsRightSideParsers
            .Add(new ObjectCreationToObjectParser<
                EntityGeneratorUpdateOperationConfiguration,
                InternalEntityGeneratorUpdateOperationConfiguration>(assignmentExpressionParer));
        availableAssignmentExpressionsRightSideParsers
            .Add(new ObjectCreationToObjectParser<
                EntityGeneratorGetByIdOperationConfiguration,
                InternalEntityGeneratorGetByIdOperationConfiguration>(assignmentExpressionParer));
        availableAssignmentExpressionsRightSideParsers
            .Add(new ObjectCreationToObjectParser<
                EntityGeneratorGetListOperationConfiguration,
                InternalEntityGeneratorGetListOperationConfiguration>(assignmentExpressionParer));
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
        INamedTypeSymbol generatorSymbol,
        out ConstructorDeclarationSyntax? constructorDeclarationSyntax)
    {
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