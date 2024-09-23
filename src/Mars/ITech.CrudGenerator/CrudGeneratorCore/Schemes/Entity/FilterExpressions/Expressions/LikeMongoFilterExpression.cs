using System.Text;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;

internal class LikeMongoFilterExpression : FilterExpression
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