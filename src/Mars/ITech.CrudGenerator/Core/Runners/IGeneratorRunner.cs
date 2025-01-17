using System.Collections.Generic;
using ITech.CrudGenerator.Core.Generators.Core;

namespace ITech.CrudGenerator.Core.Runners;

internal interface IGeneratorRunner {
    List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps);
}