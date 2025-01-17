using Teniry.CrudGenerator.Abstractions.DbContext;
using Teniry.CrudGenerator.Core.Schemes.DbContext;
using Teniry.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Core;
using Teniry.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Expressions;

namespace Teniry.CrudGenerator.Tests.Helpers;

internal class DbContextSchemeStub : DbContextScheme {
    public DbContextSchemeStub() : base(
        "",
        "",
        DbContextDbProvider.Mongo,
        new() {
            {
                FilterType.Contains, new ContainsFilterExpression()
            }, {
                FilterType.Equals, new EqualsFilterExpression()
            }, {
                FilterType.GreaterThanOrEqual, new GreaterThanOrEqualFilterExpression()
            }, {
                FilterType.LessThan, new LessThanFilterExpression()
            }, {
                FilterType.Like, new LikeFilterExpression()
            }
        }
    ) { }
}