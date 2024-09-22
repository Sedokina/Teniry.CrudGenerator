using System.Text;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Core;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Expressions;

internal class GreaterThanOrEqualFilterExpression : FilterExpression
{
    public GreaterThanOrEqualFilterExpression() : base(FilterType.GreaterThanOrEqual)
    {
    }

    public override StringBuilder Format(StringBuilder sb, string filterPropertyName, string entityPropertyToFilter)
    {
        sb.AppendLine($"if({filterPropertyName} is not null)");
        sb.AppendLine("{");
        sb.AppendLine($"query = query.Where(x => x.{entityPropertyToFilter} >= {filterPropertyName});");
        sb.AppendLine("}");

        return sb;
    }
}