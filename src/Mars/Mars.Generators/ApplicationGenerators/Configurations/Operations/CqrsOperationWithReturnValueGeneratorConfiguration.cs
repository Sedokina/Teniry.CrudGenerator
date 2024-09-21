using Mars.Generators.ApplicationGenerators.Configurations.Operations.TypedConfigurations;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;

namespace Mars.Generators.ApplicationGenerators.Configurations.Operations;

public class CqrsOperationWithReturnValueGeneratorConfiguration : CqrsOperationGeneratorConfiguration
{
    public FileTemplateBasedOperationConfiguration Dto { get; set; }

    public new CqrsOperationWithReturnValueGeneratorConfigurationBuilt Build(EntityName entityName)
    {
        base.Build()
        var built = new CqrsOperationWithReturnValueGeneratorConfigurationBuilt();
        built.Dto = new()
        {
            TemplatePath = Handler.TemplatePath,
            Name = Handler.NameConfiguration.GetName(entityName),
        };
        return built;
    }

    protected override T Build<T>(EntityName entityName)
    {
        var built = base.Build<CqrsOperationWithReturnValueGeneratorConfigurationBuilt>(entityName);
        built.Dto = new()
        {
            TemplatePath = Handler.TemplatePath,
            Name = Handler.NameConfiguration.GetName(entityName),
        };
        return built;
    }
}

public class CqrsOperationWithReturnValueGeneratorConfigurationBuilt : CqrsOperationGeneratorConfigurationBuilt
{
    public FileTemplateBasedOperationConfigurationBuilt Dto { get; set; }
}