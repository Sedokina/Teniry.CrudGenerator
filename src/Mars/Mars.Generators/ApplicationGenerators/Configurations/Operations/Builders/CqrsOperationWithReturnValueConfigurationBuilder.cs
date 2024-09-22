using Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders.TypedBuilders;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations.Builders;

internal class CqrsOperationWithReturnValueConfigurationBuilder :
    CqrsOperationWithoutReturnValueConfigurationBuilder
{
    public FileTemplateBasedOperationConfigurationBuilder Dto { get; set; } = null!;

    public new CqrsOperationWithReturnValueGeneratorConfiguration Build(EntityScheme entityScheme)
    {
        var built = new CqrsOperationWithReturnValueGeneratorConfiguration();
        Init(built, entityScheme);
        built.Dto = new()
        {
            TemplatePath = Dto.TemplatePath,
            Name = Dto.NameConfigurationBuilder.GetName(entityScheme.EntityName),
        };
        return built;
    }
}