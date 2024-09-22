using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.FilterExpressions.Core;

namespace Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Properties;

internal class EntityFilterProperty
{
    public string TypeName { get; set; }
    public string PropertyName { get; set; }
    public FilterExpression FilterExpression { get; }

    public EntityFilterProperty(
        string typeName,
        string propertyName,
        FilterExpression filterExpression)
    {
        TypeName = typeName;
        PropertyName = propertyName;
        FilterExpression = filterExpression;
    }
}