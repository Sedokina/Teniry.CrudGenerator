using System;
using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Core;
using ITech.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Expressions;
using ITech.CrudGenerator.Diagnostics;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator.Core.Schemes.DbContext;

internal class DbContextSchemeFactory
{
    public static DbContextScheme Construct(GeneratorAttributeSyntaxContext syntaxContext)
    {
        var dbContextClassSymbol = (INamedTypeSymbol)syntaxContext.TargetSymbol;
        var baseName = dbContextClassSymbol.BaseType!.Name;

        var diagnostics = new EquatableList<DiagnosticInfo>();
        if (!baseName.ToLower().EndsWith("dbcontext"))
        {
            var diagnosticInfo = new DiagnosticInfo(DiagnosticDescriptors.NotInheritedFromDbContext,
                dbContextClassSymbol.BaseType.Locations.FirstOrDefault());
            diagnostics.Add(diagnosticInfo);
        }

        var dbProviderArgument = syntaxContext.Attributes.First().ConstructorArguments.First();
        var dbProviderArgumentValue =
            dbProviderArgument.Value is null ? default : (DbContextDbProvider)dbProviderArgument.Value;

        return new DbContextScheme(
            dbContextClassSymbol.ContainingNamespace.ToString(),
            dbContextClassSymbol.Name,
            dbProviderArgumentValue,
            GetFilterExpressionsFor(dbProviderArgumentValue),
            diagnostics
        );
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