using Mars.Generators.ApplicationGenerators.Core;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class DeleteCommandGenerator : BaseGenerator<BaseCommandGeneratorConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _endpointClassName;

    public DeleteCommandGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        BaseCommandGeneratorConfiguration configuration) : base(context, symbol, configuration)
    {
        _commandName = Configuration.CommandNameConfiguration.GetName(EntityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(EntityName);
        _endpointClassName = $"Delete{EntityName}Endpoint";
    }

    public void RunGenerator()
    {
        GenerateCommand(Configuration.CommandTemplatePath);
        GenerateHandler(Configuration.HandlerTemplatePath);
        GenerateEndpoint($"{Configuration.FullConfiguration.TemplatesBasePath}.Delete.DeleteEndpoint.txt");
    }

    private void GenerateCommand(string templatePath)
    {
        var properties = PropertiesExtractor.GetPrimaryKeysOfEntityAsProperties(Symbol);
        var model = new
        {
            CommandName = _commandName,
            Properties = properties
        };
        WriteFile(templatePath, model, _commandName);
    }

    private void GenerateHandler(string templatePath)
    {
        var properties = PropertiesExtractor.GetPrimaryKeyNamesOfEntity(Symbol, "command");
        var model = new
        {
            CommandName = _commandName,
            HandlerName = _handlerName,
            FindProperties = string.Join(", ", properties)
        };

        WriteFile(templatePath, model, _handlerName);
    }
    
    private void GenerateEndpoint(string templatePath)
    {
        var model = new
        {
            CommandNamespace = PutIntoNamespace,
            PutIntoNamespace = $"Mars.Api.Endpoints.{EntityName}Endpoints",
            EndpointClassName = _endpointClassName,
            CommandName = _commandName,
        };

        WriteFile(templatePath, model, _endpointClassName);
    }
}