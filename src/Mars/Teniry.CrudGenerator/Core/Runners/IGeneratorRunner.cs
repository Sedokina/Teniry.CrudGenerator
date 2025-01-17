using System.Collections.Generic;
using Teniry.CrudGenerator.Core.Generators.Core;

namespace Teniry.CrudGenerator.Core.Runners;

internal interface IGeneratorRunner {
    List<GeneratorResult> RunGenerator(List<EndpointMap> endpointsMaps);
}