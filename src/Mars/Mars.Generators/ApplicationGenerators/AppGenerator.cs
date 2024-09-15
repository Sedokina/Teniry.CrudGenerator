using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Generators;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators;

[Generator]
public class AppGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver<GenerateCrudAttribute>());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not AttributeSyntaxReceiver<GenerateCrudAttribute> syntaxReceiver) return;

        List<(string, string)> endpointsMaps = new List<(string, string)>();
        var configuration = new CrudGeneratorConfiguration();

        foreach (var classSyntax in syntaxReceiver.Classes)
        {
            // Converting the class to semantic model to access much more meaningful data.
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            // Parse to declared symbol, so you can access each part of code separately, such as interfaces, methods, members, contructor parameters etc.
            var symbol = model.GetDeclaredSymbol(classSyntax) ?? throw new ArgumentException("symbol");

            var generateCreateCommand = new CreateCommandCrudGenerator(
                context,
                symbol,
                configuration.CreateCommandCommandGenerator);
            generateCreateCommand.RunGenerator();
            endpointsMaps.Add(generateCreateCommand.EndpointMapCall);

            var generateDeleteCommand = new DeleteCommandCrudGenerator(
                context,
                symbol,
                configuration.DeleteCommandCommandGenerator);
            generateDeleteCommand.RunGenerator();
            endpointsMaps.Add(generateDeleteCommand.EndpointMapCall);

            var generateGetByIdQuery = new GetByIdQueryCrudGenerator(
                context,
                symbol,
                configuration.GetByIdQueryGenerator);
            generateGetByIdQuery.RunGenerator();
            endpointsMaps.Add(generateGetByIdQuery.EndpointMapCall);

            var generateListQuery = new ListQueryCrudGenerator(
                context,
                symbol,
                configuration.GetListQueryGenerator);
            generateListQuery.RunGenerator();
            endpointsMaps.Add(generateListQuery.EndpointMapCall);

            var generateUpdateCommand = new UpdateCommandCrudGenerator(
                context,
                symbol,
                configuration.UpdateCommandCommandGenerator);
            generateUpdateCommand.RunGenerator();
            endpointsMaps.Add(generateUpdateCommand.EndpointMapCall);
        }

        var mapEndpointsGenerator = new MapEndpointsGenerator(context, endpointsMaps, configuration);
        mapEndpointsGenerator.RunGenerator();
    }
}

internal class MapEndpointsGenerator : BaseGenerator
{
    private readonly List<(string classNamespace, string map)> _endpointsMaps;
    private readonly CrudGeneratorConfiguration _configuration;
    private readonly string _endpointMapsClassName;

    public MapEndpointsGenerator(
        GeneratorExecutionContext context,
        List<(string, string)> endpointsMaps,
        CrudGeneratorConfiguration configuration) : base(context)
    {
        _endpointsMaps = endpointsMaps;
        _configuration = configuration;
        _endpointMapsClassName = "GeneratedEndpointsMapExtension";
    }

    public override void RunGenerator()
    {
        var usings = _endpointsMaps.Select(x => x.classNamespace).Distinct().Select(x => $"using {x};");
        var maps = _endpointsMaps.Select(x => $"app{x.map};");
        var model = new
        {
            Usings = string.Join("\n", usings),
            PutIntoNamespace = "Mars.Api",
            ExtensionClassName = _endpointMapsClassName,
            Maps = string.Join("\n\t\t", maps)
        };
        WriteFile(
            $"{_configuration.TemplatesBasePath}.GeneratedEndpointsMapExtension.txt",
            model,
            _endpointMapsClassName);
    }
}