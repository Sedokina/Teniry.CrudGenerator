using Teniry.CrudGenerator.Core.Schemes.Entity.FilterExpressions.Core;

namespace Teniry.CrudGenerator.Core.Schemes.Entity.Properties;

internal class EntityFilterProperty {
    public string TypeName { get; set; }
    public string PropertyName { get; set; }
    public FilterExpression FilterExpression { get; }

    public EntityFilterProperty(
        string typeName,
        string propertyName,
        FilterExpression filterExpression
    ) {
        TypeName = typeName;
        PropertyName = propertyName;
        FilterExpression = filterExpression;
    }
}