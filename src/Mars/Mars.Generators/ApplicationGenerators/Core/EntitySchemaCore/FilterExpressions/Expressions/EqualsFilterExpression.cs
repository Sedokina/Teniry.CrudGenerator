using System.Text;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Core;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Expressions;

public class EqualsFilterExpression : FilterExpression
{
    public EqualsFilterExpression() : base(FilterType.Equals)
    {
    }

    public override StringBuilder Format(StringBuilder sb, string filterPropertyName, string entityPropertyToFilter)
    {
        sb.AppendLine($"\tif({filterPropertyName} is not null)");
        sb.AppendLine("\t\t{");
        sb.AppendLine($"\t\t\tquery = query.Where(x => x.{entityPropertyToFilter} == {filterPropertyName});");
        sb.AppendLine("\t\t}");

        return sb;
    }
}