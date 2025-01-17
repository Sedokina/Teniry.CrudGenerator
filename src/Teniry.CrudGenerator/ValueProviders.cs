using System.Linq;
using Teniry.CrudGenerator.Abstractions.DbContext;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Teniry.CrudGenerator.Core.Schemes.DbContext;
using Teniry.CrudGenerator.Core.Schemes.InternalEntityGenerator;
using Teniry.CrudGenerator.Diagnostics;

namespace Teniry.CrudGenerator;

internal static class ValueProviders {
    private static readonly string DbContextAttributeName = typeof(UseDbContextAttribute).FullName ?? "";

    internal static IncrementalValuesProvider<Result<InternalEntityGeneratorConfiguration?>>
        GetGeneratorConfigurations(IncrementalGeneratorInitializationContext context) {
        return context.SyntaxProvider.CreateSyntaxProvider(
            (node, _) => CheckIfNodeIsInheritedFromClass(node, "EntityGeneratorConfiguration"),
            (syntaxContext, _) => TransformFoundGeneratorConfigurationsToInternalScheme(syntaxContext)
        ).WithTrackingName(CrudGeneratorTrackingNames.GetGeneratorConfigurations);
    }

    internal static IncrementalValuesProvider<Result<DbContextScheme>>
        GetDbContexts(IncrementalGeneratorInitializationContext context) {
        return context.SyntaxProvider
            .ForAttributeWithMetadataName(
                DbContextAttributeName,
                (_, _) => true,
                (syntaxContext, _) => DbContextSchemeFactory.Construct(syntaxContext)
            ).WithTrackingName(CrudGeneratorTrackingNames.GetDbContexts);
    }

    private static bool CheckIfNodeIsInheritedFromClass(SyntaxNode node, string className) {
        return node is ClassDeclarationSyntax classDeclarationSyntax &&
            classDeclarationSyntax is { BaseList.Types.Count: > 0 } &&
            classDeclarationSyntax.BaseList.Types
                .Any(
                    x => x.Type is GenericNameSyntax baseClass &&
                        baseClass.Identifier.ToString().Equals(className)
                );
    }

    private static Result<InternalEntityGeneratorConfiguration?> TransformFoundGeneratorConfigurationsToInternalScheme(
        GeneratorSyntaxContext syntaxContext
    ) {
        var declaredSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.Node);
        if (declaredSymbol is not INamedTypeSymbol namedTypeSymbol) {
            var diagnosticInfo = new DiagnosticInfo(
                DiagnosticDescriptors.WrongEntityGeneratorConfigurationSymbol,
                syntaxContext.Node.GetLocation()
            );

            return new(null, [diagnosticInfo]);
        }

        var result = InternalEntityGeneratorConfigurationFactory
            .Construct(namedTypeSymbol, syntaxContext.SemanticModel.Compilation);

        return new(result, []);
    }
}