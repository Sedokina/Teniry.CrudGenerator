using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Core;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mars.Generators.ApplicationGenerators.Core.DbContextCore;

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
            dbProviderArgumentValue,
            GetFilterExpressionsFor(dbProviderArgumentValue));
    }

    private static Dictionary<FilterType, FilterExpression> GetFilterExpressionsFor(DbContextDbProvider provider)
    {
        switch (provider)
        {
            case DbContextDbProvider.Mongo:
                return GetFilterExpressionsForMongo();
            case DbContextDbProvider.Postgres:
                return GetFilterExpressionsForPostgres();
            default:
                throw new ArgumentOutOfRangeException(nameof(provider), provider, null);
        }
    }

    private static Dictionary<FilterType, FilterExpression> GetFilterExpressionsForPostgres()
    {
        return new Dictionary<FilterType, FilterExpression>
        {
            { FilterType.Contains, new ContainsFilterExpression() },
            { FilterType.Equals, new EqualsFilterExpression() },
            { FilterType.GreaterThanOrEqual, new GreaterThanOrEqualFilterExpression() },
            { FilterType.LessThan, new LessThanFilterExpression() },
            { FilterType.Like, new LikeFilterExpression() },
        };
    }

    private static Dictionary<FilterType, FilterExpression> GetFilterExpressionsForMongo()
    {
        return new Dictionary<FilterType, FilterExpression>
        {
            { FilterType.Contains, new ContainsFilterExpression() },
            { FilterType.Equals, new EqualsFilterExpression() },
            { FilterType.GreaterThanOrEqual, new GreaterThanOrEqualFilterExpression() },
            { FilterType.LessThan, new LessThanFilterExpression() },
            { FilterType.Like, new LikeMongoFilterExpression() },
        };
    }
}