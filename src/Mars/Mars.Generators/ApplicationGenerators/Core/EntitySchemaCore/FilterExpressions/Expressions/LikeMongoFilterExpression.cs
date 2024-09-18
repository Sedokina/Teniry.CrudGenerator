using System.Text;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Core;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Expressions;

public class LikeMongoFilterExpression : FilterExpression
{
    public LikeMongoFilterExpression() : base(FilterType.Like)
    {
    }

    public override StringBuilder Format(StringBuilder sb, string filterPropertyName, string entityPropertyToFilter)
    {
        sb.AppendLine($"if({filterPropertyName} is not null)");
        sb.AppendLine("{");
        sb.AppendLine(
            $"query = query.Where(x => x.{entityPropertyToFilter}.ToLower().Contains({filterPropertyName}.ToLower()));");
        sb.AppendLine("}");

        return sb;
    }
}