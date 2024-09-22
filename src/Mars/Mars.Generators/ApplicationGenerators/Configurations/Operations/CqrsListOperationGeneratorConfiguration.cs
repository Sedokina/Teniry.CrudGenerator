using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations;

public class CqrsListOperationGeneratorConfiguration : CqrsOperationWithReturnValueGeneratorConfiguration
{
    public FileTemplateBasedOperationConfiguration Filter { get; set; }
    public FileTemplateBasedOperationConfiguration DtoListItem { get; set; }

    public new CqrsListOperationGeneratorConfigurationBuilt Build(EntityName entityName)
    {
        var built = new CqrsListOperationGeneratorConfigurationBuilt();
        Init(built, entityName);
        built.Dto = new()
        {
            TemplatePath = Dto.TemplatePath,
            Name = Dto.NameConfiguration.GetName(entityName),
        };

        built.Filter = new()
        {
            TemplatePath = Filter.TemplatePath,
            Name = Filter.NameConfiguration.GetName(entityName),
        };

        built.DtoListItem = new()
        {
            TemplatePath = DtoListItem.TemplatePath,
            Name = DtoListItem.NameConfiguration.GetName(entityName),
        };

        return built;
    }
}

public class CqrsListOperationGeneratorConfigurationBuilt : CqrsOperationWithReturnValueGeneratorConfigurationBuilt
{
    public FileTemplateBasedOperationConfigurationBuilt Filter { get; set; }
    public FileTemplateBasedOperationConfigurationBuilt DtoListItem { get; set; }
}