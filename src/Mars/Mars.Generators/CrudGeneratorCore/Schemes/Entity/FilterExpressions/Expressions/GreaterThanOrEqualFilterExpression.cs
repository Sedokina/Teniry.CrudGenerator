using System.Text;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;

namespace Mars.Generators.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;

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