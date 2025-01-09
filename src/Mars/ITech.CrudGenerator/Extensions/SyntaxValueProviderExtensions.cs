using System.Linq;
using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.Extensions;

internal static class SyntaxValueProviderExtensions
{
    private static string DbContextAttributeName = typeof(UseDbContextAttribute).FullName ?? "";

    internal static IncrementalValuesProvider<InternalEntityGeneratorConfiguration>
        CreateGeneratorConfigurationsProvider(this SyntaxValueProvider syntaxProvider)
    {
        return syntaxProvider.CreateSyntaxProvider(
            predicate: (node, _) => CheckIfNodeIsInheritedFromClass(node, "EntityGeneratorConfiguration"),
            transform: (syntaxContext, _) => TransformFoundGeneratorConfigurationsToInternalScheme(syntaxContext)
        );
    }

    internal static IncrementalValuesProvider<DbContextScheme>
        CreateDbContextConfigurationsProvider(this SyntaxValueProvider syntaxProvider)
    {
        var result = syntaxProvider
            .ForAttributeWithMetadataName(
                DbContextAttributeName,
                predicate: (_, _) => true,
                transform: (syntaxContext, _) => DbContextSchemeFactory.Construct(syntaxContext)
            );
        return result;
    }

    private static bool CheckIfNodeIsInheritedFromClass(SyntaxNode node, string className)
    {
        return node is ClassDeclarationSyntax classDeclarationSyntax &&
               classDeclarationSyntax is { BaseList.Types.Count: > 0 } &&
               classDeclarationSyntax.BaseList.Types
                   .Any(x => x.Type is GenericNameSyntax baseClass &&
                             baseClass.Identifier.ToString().Equals(className));
    }

    private static InternalEntityGeneratorConfiguration TransformFoundGeneratorConfigurationsToInternalScheme(
        GeneratorSyntaxContext syntaxContext
    )
    {
        var symbol = syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.Node) as INamedTypeSymbol;
        var result = InternalEntityGeneratorConfigurationFactory
            .Construct(symbol, syntaxContext.SemanticModel.Compilation);
        return result;
    }
}