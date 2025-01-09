using System;
using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Core;
using ITech.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Expressions;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator.Core.Schemes.DbContext;

internal class DbContextSchemeFactory
{
    public static DbContextScheme Construct(GeneratorAttributeSyntaxContext syntaxContext)
    {
        var dbContextClassSymbol = (INamedTypeSymbol)syntaxContext.TargetSymbol;
        var baseName = dbContextClassSymbol.BaseType!.Name;

        if (!baseName.ToLower().EndsWith("dbcontext"))
        {
            throw new Exception(
                $"{nameof(UseDbContextAttribute)} used on class {baseName}, but it is not DbContext class. If it is DbContext class add postfix DbContext to class name");
        }

        var dbProviderArgument = syntaxContext.Attributes.First().ConstructorArguments.First();
        var dbProviderArgumentValue =
            dbProviderArgument.Value is null ? default : (DbContextDbProvider)dbProviderArgument.Value;

        return new DbContextScheme(
            dbContextClassSymbol.ContainingNamespace.ToString(),
            dbContextClassSymbol.Name,
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