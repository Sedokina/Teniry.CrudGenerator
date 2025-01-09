using System.Collections.Generic;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;

namespace ITech.CrudGenerator.CrudGeneratorCore.GeneratorRunners;

internal interface IGeneratorRunner
{
    List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps);
}