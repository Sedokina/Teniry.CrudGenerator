using ITech.CrudGenerator.Abstractions.DbContext;
using ITech.CrudGenerator.Core.Schemes.DbContext;
using ITech.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Core;
using ITech.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Expressions;

namespace ITech.CrudGenerator.Tests.Helpers;

internal class DbContextSchemeStub : DbContextScheme
{
    public DbContextSchemeStub() : base("", "", DbContextDbProvider.Mongo,
        new Dictionary<FilterType, FilterExpression>
        {
            {
                FilterType.Contains, new ContainsFilterExpression()
            },
            {
                FilterType.Equals, new EqualsFilterExpression()
            },
            {
                FilterType.GreaterThanOrEqual, new GreaterThanOrEqualFilterExpression()
            },
            {
                FilterType.LessThan, new LessThanFilterExpression()
            },
            {
                FilterType.Like, new LikeFilterExpression()
            }
        }, [])
    {
    }
}