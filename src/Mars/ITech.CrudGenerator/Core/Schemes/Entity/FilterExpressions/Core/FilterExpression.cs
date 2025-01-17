using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ITech.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Core;

public abstract class FilterExpression {
    public FilterType FilterType { get; private set; }

    public FilterExpression(FilterType filterType) {
        FilterType = filterType;
    }

    public abstract StatementSyntax BuildExpression(string filterPropertyName, string entityPropertyToFilter);
}