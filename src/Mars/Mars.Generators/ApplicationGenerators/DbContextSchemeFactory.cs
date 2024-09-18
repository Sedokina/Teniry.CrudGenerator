using System;
using System.Linq;
using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators.ApplicationGenerators;

public class DbContextSchemeFactory
{
    public static DbContextScheme Construct(GeneratorExecutionContext context)
    {
        var attributeName = nameof(UseDbContextAttribute).Replace("Attribute", "");
        var foundAttributes = context.Compilation
            .SyntaxTrees
            .SelectMany(x => x.GetRoot().DescendantNodes())
            .Where(x => x is AttributeSyntax)
            .Cast<AttributeSyntax>()
            .Where(x => x.Name.ToString().Contains(attributeName)).ToList();

        if (foundAttributes.Count == 0)
        {
            throw new Exception(
                $"Usage of attribute {nameof(UseDbContextAttribute)} not found. Use this attribute on DbContext class for generator to have access to DbContext");
        }

        var useDbContextAttribute = foundAttributes.First();

        var dbContextClass = (useDbContextAttribute.Parent as AttributeListSyntax)?.Parent as ClassDeclarationSyntax;
        if (dbContextClass is null)
        {
            throw new Exception(
                $"Class of {nameof(UseDbContextAttribute)} not found, may be attribute applied not to a class");
        }

        var dbContextClassSemanticModel = context.Compilation.GetSemanticModel(dbContextClass!.SyntaxTree);
        var dbContextClassSymbol = (INamedTypeSymbol)dbContextClassSemanticModel.GetDeclaredSymbol(dbContextClass);
        var baseName = dbContextClassSymbol!.BaseType!.Name;

        if (!baseName.ToLower().EndsWith("dbcontext"))
        {
            throw new Exception(
                $"{nameof(UseDbContextAttribute)} used on class {baseName}, but it is not DbContext class. If it is DbContext class add postfix DbContext to class name");
        }

        var dbProviderArgument = useDbContextAttribute.ArgumentList!.Arguments.First();

        var dbProviderArgumentSemanticModel = context.Compilation.GetSemanticModel(dbProviderArgument.SyntaxTree);
        var dbProviderArgumentValue = (DbContextDbProvider)dbProviderArgumentSemanticModel
            .GetOperation(dbProviderArgument.Expression)!.ConstantValue.Value;
        return new DbContextScheme(
            dbContextClassSymbol.ContainingNamespace.ToString(),
            dbContextClassSymbol!.Name,
            dbProviderArgumentValue);
    }
}