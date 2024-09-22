using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;

public class CqrsListOperationConfigurationBuilder : CqrsOperationWithReturnValueConfigurationBuilder
{
    public FileTemplateBasedOperationConfigurationBuilder Filter { get; set; }
    public FileTemplateBasedOperationConfigurationBuilder DtoListItem { get; set; }

    public new CqrsListOperationGeneratorConfiguration Build(EntityName entityName)
    {
        var built = new CqrsListOperationGeneratorConfiguration();
        Init(built, entityName);
        built.Dto = new()
        {
            TemplatePath = Dto.TemplatePath,
            Name = Dto.NameConfigurationBuilder.GetName(entityName),
        };

        built.Filter = new()
        {
            TemplatePath = Filter.TemplatePath,
            Name = Filter.NameConfigurationBuilder.GetName(entityName),
        };

        built.DtoListItem = new()
        {
            TemplatePath = DtoListItem.TemplatePath,
            Name = DtoListItem.NameConfigurationBuilder.GetName(entityName),
        };

        return built;
    }
}