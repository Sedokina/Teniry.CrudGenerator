using System;
using System.Collections.Generic;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Generators;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators;

[Generator]
public class CrudGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver<GenerateCrudAttribute>());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not AttributeSyntaxReceiver<GenerateCrudAttribute> syntaxReceiver) return;

        List<string> endpointMaps = new List<string>();
        foreach (var classSyntax in syntaxReceiver.Classes)
        {
            // Converting the class to semantic model to access much more meaningful data.
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            // Parse to declared symbol, so you can access each part of code separately, such as interfaces, methods, members, contructor parameters etc.
            var symbol = model.GetDeclaredSymbol(classSyntax) ?? throw new ArgumentException("symbol");

            var configuration = new CrudGeneratorConfiguration();

            var generateCreateCommand = new CreateCommandGenerator(
                context,
                symbol,
                configuration.CreateCommandCommandGenerator);
            generateCreateCommand.RunGenerator();
            endpointMaps.Add(generateCreateCommand.EndpointMapCall);

            var generateDeleteCommand = new DeleteCommandGenerator(
                context,
                symbol,
                configuration.DeleteCommandCommandGenerator);
            generateDeleteCommand.RunGenerator();
            endpointMaps.Add(generateDeleteCommand.EndpointMapCall);

            var generateGetByIdQuery = new GetByIdQueryGenerator(
                context,
                symbol,
                configuration.GetByIdQueryGenerator);
            generateGetByIdQuery.RunGenerator();
            endpointMaps.Add(generateGetByIdQuery.EndpointMapCall);

            var generateListQuery = new ListQueryGenerator(
                context,
                symbol,
                configuration.GetListQueryGenerator);
            generateListQuery.RunGenerator();
            endpointMaps.Add(generateListQuery.EndpointMapCall);

            var generateUpdateCommand = new UpdateCommandGenerator(
                context,
                symbol,
                configuration.UpdateCommandCommandGenerator);
            generateUpdateCommand.RunGenerator();
            endpointMaps.Add(generateUpdateCommand.EndpointMapCall);
        }
    }
}