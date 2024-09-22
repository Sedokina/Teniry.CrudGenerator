using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;

internal class CqrsListOperationConfigurationBuilder : CqrsOperationWithReturnValueConfigurationBuilder
{
    public FileTemplateBasedOperationConfigurationBuilder Filter { get; set; } = null!;
    public FileTemplateBasedOperationConfigurationBuilder DtoListItem { get; set; } = null!;

    public new CqrsListOperationGeneratorConfiguration Build(EntityScheme entityScheme)
    {
        var built = new CqrsListOperationGeneratorConfiguration();
        Init(built, entityScheme);
        built.Dto = new()
        {
            TemplatePath = Dto.TemplatePath,
            Name = Dto.NameConfigurationBuilder.GetName(entityScheme.EntityName),
        };

        built.Filter = new()
        {
            TemplatePath = Filter.TemplatePath,
            Name = Filter.NameConfigurationBuilder.GetName(entityScheme.EntityName),
        };

        built.DtoListItem = new()
        {
            TemplatePath = DtoListItem.TemplatePath,
            Name = DtoListItem.NameConfigurationBuilder.GetName(entityScheme.EntityName),
        };

        return built;
    }
}