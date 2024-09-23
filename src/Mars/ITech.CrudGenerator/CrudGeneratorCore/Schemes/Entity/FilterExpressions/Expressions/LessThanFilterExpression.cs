using System.Text;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;

internal class LessThanFilterExpression : FilterExpression
{
    public LessThanFilterExpression() : base(FilterType.LessThan)
    {
    }

    public override StringBuilder Format(StringBuilder sb, string filterPropertyName, string entityPropertyToFilter)
    {
        sb.AppendLine($"if({filterPropertyName} is not null)");
        sb.AppendLine("{");
        sb.AppendLine($"query = query.Where(x => x.{entityPropertyToFilter} < {filterPropertyName});");
        sb.AppendLine("}");

        return sb;
    }
}