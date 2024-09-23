using System.Collections.Generic;
using ITech.CrudGenerator.Abstractions;
using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;

internal class DbContextScheme
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