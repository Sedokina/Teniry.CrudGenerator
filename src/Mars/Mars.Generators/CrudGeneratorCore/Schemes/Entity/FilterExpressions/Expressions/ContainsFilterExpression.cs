using System.Text;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Core;

namespace Mars.Generators.CrudGeneratorCore.Schemes.Entity.FilterExpressions.Expressions;

internal class ContainsFilterExpression : FilterExpression
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