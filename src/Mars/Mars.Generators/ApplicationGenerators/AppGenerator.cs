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

        List<EndpointMap> endpointsMaps = new();
        var configuration = new CrudGeneratorConfiguration();

        foreach (var classSyntax in syntaxReceiver.Classes)
        {
            // Converting the class to semantic model to access much more meaningful data.
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            // Parse to declared symbol, so you can access each part of code separately, such as interfaces, methods, members, contructor parameters etc.
            var symbol = model.GetDeclaredSymbol(classSyntax) ?? throw new ArgumentException("symbol");

            var entityConfiguration = new EntityConfiguration
            {
                Title = EntityConfiguration.GetTitleFromEntityName(symbol.Name)
            };

            var generateGetByIdQuery = new GetByIdQueryCrudGenerator(
                context,
                symbol,
                configuration.GetByIdQueryGenerator,
                entityConfiguration);
            generateGetByIdQuery.RunGenerator();
            endpointsMaps.Add(generateGetByIdQuery.EndpointMap);

            var generateListQuery = new ListQueryCrudGenerator(
                context,
                symbol,
                configuration.GetListQueryGenerator,
                entityConfiguration);
            generateListQuery.RunGenerator();
            endpointsMaps.Add(generateListQuery.EndpointMap);

            var generateCreateCommand = new CreateCommandCrudGenerator(
                context,
                symbol,
                configuration.CreateCommandCommandGenerator,
                entityConfiguration);
            generateCreateCommand.RunGenerator();
            endpointsMaps.Add(generateCreateCommand.EndpointMap);

            var generateUpdateCommand = new UpdateCommandCrudGenerator(
                context,
                symbol,
                configuration.UpdateCommandCommandGenerator,
                entityConfiguration);
            generateUpdateCommand.RunGenerator();
            endpointsMaps.Add(generateUpdateCommand.EndpointMap);

            var generateDeleteCommand = new DeleteCommandCrudGenerator(
                context,
                symbol,
                configuration.DeleteCommandCommandGenerator,
                entityConfiguration);
            generateDeleteCommand.RunGenerator();
            endpointsMaps.Add(generateDeleteCommand.EndpointMap);
        }

        var mapEndpointsGenerator = new MapEndpointsGenerator(context, endpointsMaps, configuration);
        mapEndpointsGenerator.RunGenerator();
    }
}

internal class MapEndpointsGenerator : BaseGenerator
{
    private readonly List<EndpointMap> _endpointsMaps;
    private readonly CrudGeneratorConfiguration _configuration;
    private readonly string _endpointMapsClassName;

    public MapEndpointsGenerator(
        GeneratorExecutionContext context,
        List<EndpointMap> endpointsMaps,
        CrudGeneratorConfiguration configuration) : base(context)
    {
        _endpointsMaps = endpointsMaps;
        _configuration = configuration;
        _endpointMapsClassName = "GeneratedEndpointsMapExtension";
    }

    public override void RunGenerator()
    {
        var usings = _endpointsMaps.Select(x => x.EndpointNamespace).Distinct().Select(x => $"using {x};");
        var maps = _endpointsMaps
            .Select(x =>
                $"app.Map{x.HttpMethod}(\"{x.EndpointRoute}\", {x.FunctionCall}).WithTags(\"{x.EntityName}\");")
            .ToList();
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