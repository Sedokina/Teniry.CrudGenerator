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
        sb.AppendLine($"\tif({filterPropertyName} is not null)");
        sb.AppendLine("\t\t{");
        sb.AppendLine($"\t\t\tquery = query.Where(x => {entityPropertyToFilter}.Contains(x.{filterPropertyName}));");
        sb.AppendLine("\t\t}");

        return sb;
    }
}