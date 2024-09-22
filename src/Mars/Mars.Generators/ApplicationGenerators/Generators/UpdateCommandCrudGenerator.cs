using Mars.Generators.ApplicationGenerators.Configurations.Operations;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore.Formatters;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators.Generators;

internal class UpdateCommandCrudGenerator : BaseCrudGenerator<CqrsOperationWithoutReturnValueGeneratorConfiguration>
{
    private readonly string _commandName;
    private readonly string _handlerName;
    private readonly string _vmName;
    private readonly string _endpointClassName;

    public UpdateCommandCrudGenerator(
        GeneratorExecutionContext context,
        CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration> scheme) : base(context, scheme)
    {
        _commandName = scheme.Configuration.Operation.Name;
        _handlerName = scheme.Configuration.Handler.Name;
        _vmName = $"Update{EntityScheme.EntityName}Vm";
        _endpointClassName = scheme.Configuration.Endpoint.Name;
    }

    public override void RunGenerator()
    {
        GenerateCommand(Scheme.Configuration.Operation.TemplatePath);
        GenerateHandler(Scheme.Configuration.Handler.TemplatePath);
        GenerateViewModel($"{Scheme.Configuration.GlobalConfiguration.TemplatesBasePath}.Update.UpdateVm.txt");
        GenerateEndpoint(Scheme.Configuration.Endpoint.TemplatePath);
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

        EndpointMap = new EndpointMap(EntityScheme.EntityName.ToString(),
            Scheme.Configuration.OperationsSharedConfiguration.EndpointsNamespaceForFeature,
            "Put",
            Scheme.Configuration.Endpoint.Route,
            $"{_endpointClassName}.{Scheme.Configuration.Endpoint.FunctionName}");
    }
}