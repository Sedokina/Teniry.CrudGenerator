using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations;

public class CqrsOperationWithReturnValueGeneratorConfiguration : CqrsOperationGeneratorConfiguration
{
    public FileTemplateBasedOperationConfiguration Dto { get; set; }

    public new CqrsOperationWithReturnValueGeneratorConfigurationBuilt Build(EntityName entityName)
    {
        var built = new CqrsOperationWithReturnValueGeneratorConfigurationBuilt();
        Init(built, entityName);
        built.Dto = new()
        {
            TemplatePath = Dto.TemplatePath,
            Name = Dto.NameConfiguration.GetName(entityName),
        };
        return built;
    }
}

public class CqrsOperationWithReturnValueGeneratorConfigurationBuilt : CqrsOperationGeneratorConfigurationBuilt
{
    public FileTemplateBasedOperationConfigurationBuilt Dto { get; set; }
}