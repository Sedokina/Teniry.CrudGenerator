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
        sb.AppendLine($"if({filterPropertyName} is not null)");
        sb.AppendLine("{");
        sb.AppendLine(
            $"query = query.Where(x => EF.Functions.Like(x.{entityPropertyToFilter}, $\"%{{{filterPropertyName}}}%\"));");
        sb.AppendLine("}");

        return sb;
    }
}