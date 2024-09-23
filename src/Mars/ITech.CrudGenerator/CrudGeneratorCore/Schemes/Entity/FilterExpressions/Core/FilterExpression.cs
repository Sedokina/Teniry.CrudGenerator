using System.Text;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;

public abstract class FilterExpression
{
    public FilterType FilterType { get; private set; }

    public FilterExpression(FilterType filterType)
    {
        FilterType = filterType;
    }

    public abstract StringBuilder Format(StringBuilder sb, string filterPropertyName, string entityPropertyToFilter);
}