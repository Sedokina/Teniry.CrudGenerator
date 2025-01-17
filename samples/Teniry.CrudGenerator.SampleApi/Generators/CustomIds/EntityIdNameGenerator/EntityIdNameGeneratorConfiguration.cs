using Teniry.CrudGenerator.Abstractions.Configuration;

namespace Teniry.CrudGenerator.SampleApi.Generators.CustomIds.EntityIdNameGenerator;

public class EntityIdNameGeneratorConfiguration : EntityGeneratorConfiguration<EntityIdName> { }

public class EntityIdName {
    public Guid EntityIdNameId { get; set; }
    public string Name { get; set; } = "";
}