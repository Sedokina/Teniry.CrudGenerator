using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal interface IConfigurationBuilderFactory
{
    public object Construct(
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration,
        CqrsOperationsSharedConfigurationBuilder operationsSharedConfiguration,
        InternalEntityGeneratorConfiguration internalEntityGeneratorConfiguration
    );
}