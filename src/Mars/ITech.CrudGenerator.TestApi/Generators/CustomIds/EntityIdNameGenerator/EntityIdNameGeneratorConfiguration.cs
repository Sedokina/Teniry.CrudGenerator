using ITech.CrudGenerator.Abstractions.Configuration;

namespace ITech.CrudGenerator.TestApi.Generators.CustomIds.EntityIdNameGenerator;

public class EntityIdNameGeneratorConfiguration : EntityGeneratorConfiguration<EntityIdName>
{
}

public class EntityIdName
{
    public Guid EntityIdNameId { get; set; }
    public string Name { get; set; } = "";
}