using System.Collections.Generic;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;

internal interface IConfigurationBuilderFactory
{
    List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps);
}