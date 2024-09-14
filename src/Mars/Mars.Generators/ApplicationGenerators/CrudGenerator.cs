using System;
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

        foreach (var classSyntax in syntaxReceiver.Classes)
        {
            // Converting the class to semantic model to access much more meaningful data.
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            // Parse to declared symbol, so you can access each part of code separately, such as interfaces, methods, members, contructor parameters etc.
            var symbol = model.GetDeclaredSymbol(classSyntax) ?? throw new ArgumentException("symbol");

            var generateCreateCommand = new CreateCommandGenerator(context, symbol);
            generateCreateCommand.RunGenerator();

            var generateDeleteCommand = new DeleteCommandGenerator(context, symbol);
            generateDeleteCommand.RunGenerator();

            var generateGetByIdQuery = new GetByIdQueryGenerator(context, symbol);
            generateGetByIdQuery.RunGenerator();

            var generateListQuery = new ListQueryGenerator(context, symbol);
            generateListQuery.RunGenerator();

            var generateUpdateCommand = new UpdateCommandGenerator(context, symbol);
            generateUpdateCommand.RunGenerator();
        }
    }
}