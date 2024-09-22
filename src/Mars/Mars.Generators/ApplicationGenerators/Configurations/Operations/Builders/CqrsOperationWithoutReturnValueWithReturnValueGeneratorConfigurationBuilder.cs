using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;

public class CqrsOperationWithoutReturnValueWithReturnValueGeneratorConfigurationBuilder : CqrsOperationWithoutReturnValueGeneratorConfigurationBuilder
{
    public FileTemplateBasedOperationConfigurationBuilder Dto { get; set; }

    public new CqrsOperationWithoutReturnValueWithReturnValueGeneratorConfiguration Build(EntityName entityName)
    {
        var built = new CqrsOperationWithoutReturnValueWithReturnValueGeneratorConfiguration();
        Init(built, entityName);
        built.Dto = new()
        {
            TemplatePath = Dto.TemplatePath,
            Name = Dto.NameConfigurationBuilder.GetName(entityName),
        };
        return built;
    }
}