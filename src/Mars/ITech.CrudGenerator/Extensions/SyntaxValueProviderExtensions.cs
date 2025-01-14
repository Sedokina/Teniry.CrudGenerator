using System.Linq;
using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.Core.Schemes.DbContext;
using ITech.CrudGenerator.Core.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.Extensions;

internal static class SyntaxValueProviderExtensions
{
    private static readonly string DbContextAttributeName = typeof(UseDbContextAttribute).FullName ?? "";

    internal static IncrementalValuesProvider<Result<InternalEntityGeneratorConfiguration?>>
        CreateGeneratorConfigurationsProvider(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
            predicate: (node, _) => CheckIfNodeIsInheritedFromClass(node, "EntityGeneratorConfiguration"),
            transform: (syntaxContext, _) => TransformFoundGeneratorConfigurationsToInternalScheme(syntaxContext)
        ).WithTrackingName("GeneratorConfigurationsProviders");
    }

    internal static IncrementalValuesProvider<Result<DbContextScheme>>
        CreateDbContextConfigurationsProvider(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider
            .ForAttributeWithMetadataName(
                DbContextAttributeName,
                predicate: (_, _) => true,
                transform: (syntaxContext, _) => DbContextSchemeFactory.Construct(syntaxContext)
            ).WithTrackingName("DbContextSchemeProviders");
    }

    private static bool CheckIfNodeIsInheritedFromClass(SyntaxNode node, string className)
    {
        return node is ClassDeclarationSyntax classDeclarationSyntax &&
               classDeclarationSyntax is { BaseList.Types.Count: > 0 } &&
               classDeclarationSyntax.BaseList.Types
                   .Any(x => x.Type is GenericNameSyntax baseClass &&
                             baseClass.Identifier.ToString().Equals(className));
    }

    private static Result<InternalEntityGeneratorConfiguration?> TransformFoundGeneratorConfigurationsToInternalScheme(
        GeneratorSyntaxContext syntaxContext
    )
    {
        var declaredSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.Node);
        if (declaredSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            var diagnosticInfo = new DiagnosticInfo(DiagnosticDescriptors.WrongEntityGeneratorConfigurationSymbol,
                syntaxContext.Node.GetLocation());
            return new(null, [diagnosticInfo]);
        }

        var result = InternalEntityGeneratorConfigurationFactory
            .Construct(namedTypeSymbol, syntaxContext.SemanticModel.Compilation);
        return new(result, []);
    }
}