using System.Text;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Core;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Expressions;

public class ContainsFilterExpression : FilterExpression
{
    public ContainsFilterExpression() : base(FilterType.Contains)
    {
    }

    public override StringBuilder Format(StringBuilder sb, string filterPropertyName, string entityPropertyToFilter)
    {
        sb.AppendLine($"if({filterPropertyName} is not null && {filterPropertyName}.Length > 0)");
        sb.AppendLine("{");
        sb.AppendLine($"query = query.Where(x => {entityPropertyToFilter}.Contains(x.{filterPropertyName}));");
        sb.AppendLine("}");

        return sb;
    }
}