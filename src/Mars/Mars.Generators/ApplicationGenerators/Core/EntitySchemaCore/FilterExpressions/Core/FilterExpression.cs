using System.Text;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Core;

public abstract class FilterExpression
{
    public FilterType FilterType { get; private set; }

    public FilterExpression(FilterType filterType)
    {
        FilterType = filterType;
    }

    public abstract StringBuilder Format(StringBuilder sb, string filterPropertyName, string entityPropertyToFilter);
}