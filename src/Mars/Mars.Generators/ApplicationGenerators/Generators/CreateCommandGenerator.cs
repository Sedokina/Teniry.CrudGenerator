using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class CreateCommandGenerator : BaseGenerator
{
    private readonly string _commandName;
    private readonly string _handlerName;

    public CreateCommandGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        CrudGeneratorConfiguration configuration) 
        : base(context, symbol, configuration, configuration.CreateCommandCommandGenerator.FunctionName)
    {
        _commandName = Configuration.CreateCommandCommandGenerator.CommandNameFormat.GetName(_entityName);
        _handlerName = Configuration.CreateCommandCommandGenerator.HandlerNameFormat.GetName(_entityName);
    }

    public void RunGenerator()
    {
        GenerateCommand(Configuration.CreateCommandCommandGenerator.CommandTemplatePath);
        GenerateHandler(Configuration.CreateCommandCommandGenerator.HandlerTemplatePath);
    }

    private void GenerateCommand(string templatePath)
    {
        var properties = PropertiesExtractor.GetAllPropertiesOfEntity(Symbol, true);
        var model = new
        {
            CommandName = _commandName,
            Properties = properties
        };

        WriteFile(templatePath, model, _commandName);
    }

    private void GenerateHandler(string templatePath)
    {
        var model = new
        {
            CommandName = _commandName,
            HandlerName = _handlerName
        };

        WriteFile(templatePath, model, _handlerName);
    }
}