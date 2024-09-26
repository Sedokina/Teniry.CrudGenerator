using System.Text;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;

internal class LikeFilterExpression : FilterExpression
{
    public LikeFilterExpression() : base(FilterType.Like)
    {
    }

    public override StringBuilder Format(StringBuilder sb, string filterPropertyName, string entityPropertyToFilter)
    {
        sb.AppendLine($"if({filterPropertyName} is not null)");
        sb.AppendLine("{");
        sb.AppendLine(
            $"query = query.Where(x => EF.Functions.ILike(x.{entityPropertyToFilter}, $\"%{{{filterPropertyName}}}%\"));");
        sb.AppendLine("}");

        return sb;
    }
}