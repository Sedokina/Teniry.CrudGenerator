using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.CrudGeneratorCore.ConfigurationsReceiver;

internal class EntityGeneratorConfigurationSyntaxReceiver : ISyntaxReceiver
{
    public IList<ClassForCrudGeneration> ClassesForCrudGeneration { get; } = [];

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
            IsInheritedFrom(classDeclarationSyntax, TypeNamesForAnalyzers.EntityGeneratorConfiguration))
        {
            var baseTypeSyntax = classDeclarationSyntax.BaseList!.Types
                .First(x => x.Type is GenericNameSyntax baseClass &&
                            baseClass.Identifier.ToString().Equals(TypeNamesForAnalyzers.EntityGeneratorConfiguration));
            var entityClass = (baseTypeSyntax.Type as GenericNameSyntax)!.TypeArgumentList.Arguments.First();
            ClassesForCrudGeneration.Add(new(classDeclarationSyntax, (IdentifierNameSyntax)entityClass));
        }
    }

    private bool IsInheritedFrom(ClassDeclarationSyntax classDeclarationSyntax, string className)
    {
        return classDeclarationSyntax is { BaseList.Types.Count: > 0 } &&
               classDeclarationSyntax.BaseList.Types
                   .Any(x => x.Type is GenericNameSyntax baseClass &&
                             baseClass.Identifier.ToString().Equals(className));
    }
}

internal class ClassForCrudGeneration(
    ClassDeclarationSyntax entityGeneratorDeclaration,
    IdentifierNameSyntax entityDeclaration)
{
    public (ISymbol EntityGeneratorConfigurationSymbol, ITypeSymbol EntitySymbol)
        AsSymbol(GeneratorExecutionContext context)
    {
        // Converting the class to semantic model to access much more meaningful data.
        var model = context.Compilation.GetSemanticModel(entityGeneratorDeclaration.SyntaxTree);
        // Parse to declared symbol, to access each part of code separately,
        // such as interfaces, methods, members, contructor parameters etc.
        var entityGeneratorConfigurationSymbol = model.GetDeclaredSymbol(entityGeneratorDeclaration) ??
                                                 throw new ArgumentException("symbol");

        // Parse to type symbol, to access each part of code separately,
        // such as interfaces, methods, members, contructor parameters etc.
        var entitySymbol = model.GetTypeInfo(entityDeclaration).Type;
        return (entityGeneratorConfigurationSymbol, entitySymbol!);
    }
}