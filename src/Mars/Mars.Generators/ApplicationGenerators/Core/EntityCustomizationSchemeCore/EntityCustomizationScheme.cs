namespace Mars.Generators.ApplicationGenerators.Core.EntityCustomizationSchemeCore;

internal class EntityCustomizationScheme
{
    public string? Title { get; set; }
    public string? TitlePlural { get; set; }
    public EntityCustomizationSchemeDefaultSort? DefaultSort { get; set; }
}

internal class EntityCustomizationSchemeDefaultSort(string direction, string propertyName)
{
    public string Direction { get; set; } = direction;
    public string PropertyName { get; set; } = propertyName;
}