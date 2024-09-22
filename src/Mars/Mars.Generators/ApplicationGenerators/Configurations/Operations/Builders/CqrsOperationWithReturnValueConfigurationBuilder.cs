using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;

public class
    CqrsOperationWithReturnValueConfigurationBuilder :
    CqrsOperationWithoutReturnValueConfigurationBuilder
{
    public FileTemplateBasedOperationConfigurationBuilder Dto { get; set; }

    public new CqrsOperationWithReturnValueGeneratorConfiguration Build(EntityName entityName)
    {
        var built = new CqrsOperationWithReturnValueGeneratorConfiguration();
        Init(built, entityName);
        built.Dto = new()
        {
            TemplatePath = Dto.TemplatePath,
            Name = Dto.NameConfigurationBuilder.GetName(entityName),
        };
        return built;
    }
}