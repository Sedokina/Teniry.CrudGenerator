using System.Text;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Core;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Expressions;

public class LikeFilterExpression : FilterExpression
{
    public LikeFilterExpression() : base(FilterType.Like)
    {
    }

    public override StringBuilder Format(StringBuilder sb, string filterPropertyName, string entityPropertyToFilter)
    {
        sb.AppendLine($"\tif({filterPropertyName} is not null)");
        sb.AppendLine("\t\t{");
        sb.AppendLine(
            $"\t\t\tquery = query.Where(x => EF.Functions.Like(x.{entityPropertyToFilter}, $\"%{{{filterPropertyName}}}%\"));");
        sb.AppendLine("\t\t}");

        return sb;
    }
}