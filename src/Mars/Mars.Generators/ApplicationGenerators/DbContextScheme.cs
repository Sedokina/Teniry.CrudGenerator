using System;
using System.Collections.Generic;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Core;

namespace Mars.Generators.ApplicationGenerators;

public class DbContextScheme
{
    private readonly Dictionary<FilterType, FilterExpression> _filterExpressions;
    public string DbContextNamespace { get; set; }
    public string DbContextName { get; set; }
    public DbContextDbProvider Provider { get; set; }

    public DbContextScheme(
        string dbContextNamespace,
        string dbContextName,
        DbContextDbProvider provider,
        Dictionary<FilterType, FilterExpression> filterExpressions)
    {
        _filterExpressions = filterExpressions;
        DbContextNamespace = dbContextNamespace;
        DbContextName = dbContextName;
        Provider = provider;
    }

    public FilterExpression GetFilterExpression(FilterType filterType)
    {
        return _filterExpressions[filterType];
    }
}