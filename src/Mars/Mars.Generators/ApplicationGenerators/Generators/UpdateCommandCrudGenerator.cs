using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

public class UpdateCommandCrudGenerator : BaseCrudGenerator<BaseCommandGeneratorConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _vmName;
    private readonly string _endpointClassName;

    public UpdateCommandCrudGenerator(
        GeneratorExecutionContext context,
        ISymbol symbol,
        BaseCommandGeneratorConfiguration configuration,
        EntityScheme entityScheme) : base(context, symbol, configuration, entityScheme)
    {
        _commandName = Configuration.CommandNameConfiguration.GetName(EntityScheme.EntityName);
        _handlerName = Configuration.HandlerNameConfiguration.GetName(EntityScheme.EntityName);
        _vmName = $"Update{EntityScheme.EntityName}Vm";
        _endpointClassName = Configuration.EndpointNameConfiguration.GetName(EntityScheme.EntityName);
    }

    public override void RunGenerator()
    {
        GenerateCommand(Configuration.CommandTemplatePath);
        GenerateHandler(Configuration.HandlerTemplatePath);
        GenerateViewModel($"{Configuration.FullConfiguration.TemplatesBasePath}.Update.UpdateVm.txt");
        GenerateEndpoint(Configuration.EndpointTemplatePath);
    }

    private void GenerateCommand(string templatePath)
    {
        var properties = EntityScheme.Properties.FormatAsProperties();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorBody = EntityScheme.PrimaryKeys.FormatAsConstructorBody();
        var model = new
        {
            CommandName = _commandName,
            Properties = properties,
            ConstructorParameters = constructorParameters,
            ConstructorBody = constructorBody
        };

        WriteFile(templatePath, model, _commandName);
    }

    private void GenerateHandler(string templatePath)
    {
        var findParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters("command");
        var model = new
        {
            CommandName = _commandName,
            HandlerName = _handlerName,
            FindParameters = findParameters
        };

        WriteFile(templatePath, model, _handlerName);
    }

    private void GenerateViewModel(string templatePath)
    {
        var properties = EntityScheme.NotPrimaryKeys.FormatAsProperties();
        var model = new
        {
            VmName = _vmName,
            Properties = properties,
        };

        WriteFile(templatePath, model, _vmName);
    }

    private void GenerateEndpoint(string templatePath)
    {
        var routeParams = EntityScheme.PrimaryKeys.FormatAsMethodDeclarationParameters();
        var constructorParameters = EntityScheme.PrimaryKeys.FormatAsMethodCallParameters();
        var model = new
        {
            EndpointClassName = _endpointClassName,
            RouteParams = routeParams,
            VmName = _vmName,
            CommandName = _commandName,
            CommandConstructorParameters = constructorParameters
        };

        WriteFile(templatePath, model, _endpointClassName);

        var constructorParametersForRoute = EntityScheme.PrimaryKeys.GetAsMethodCallParameters();
        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            EndpointNamespace,
            "Put",
            Configuration.EndpointRouteConfiguration
                .GetRoute(EntityScheme.EntityName.ToString(), constructorParametersForRoute),
            $"{_endpointClassName}.{Configuration.EndpointRouteConfiguration.FunctionName}");
    }
}