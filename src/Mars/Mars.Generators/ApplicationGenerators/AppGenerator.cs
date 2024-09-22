using System.Collections.Generic;
using System.Linq;
using Mars.Generators.ApplicationGenerators.Configurations.Global;
using Mars.Generators.ApplicationGenerators.Configurations.Global.Factories;
using Mars.Generators.ApplicationGenerators.Configurations.Operations;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.ApplicationGenerators.Configurations.Operations.Factories;
using Mars.Generators.ApplicationGenerators.Core;
using Mars.Generators.ApplicationGenerators.Core.DbContextCore;
using Mars.Generators.ApplicationGenerators.Core.EntityCustomizationSchemeCore;
using Mars.Generators.ApplicationGenerators.Core.EntitySchemaCore;
using Mars.Generators.ApplicationGenerators.Generators;
using Microsoft.CodeAnalysis;

namespace Mars.Generators.ApplicationGenerators;

[Generator]
public class AppGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new EntityGeneratorConfigurationSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not EntityGeneratorConfigurationSyntaxReceiver syntaxReceiver) return;

        var dbContextScheme = DbContextSchemeFactory.Construct(context);

        List<EndpointMap> endpointsMaps = new();
        var globalConfiguration = GlobalCrudGeneratorConfigurationDefaultConfigurationFactory.Construct();
        var sharedConfiguration = SharedCqrsOperationDefaultConfigurationFactory.Construct();

        foreach (var classSyntax in syntaxReceiver.ClassesForCrudGeneration)
        {
            var (entityGeneratorConfigurationSymbol, entitySymbol) = classSyntax.AsSymbol(context);

            var entityCustomizationScheme = EntityCastomizationSchemeFactory
                .Construct(entityGeneratorConfigurationSymbol as INamedTypeSymbol, context);
            var entityScheme = EntitySchemeFactory.Construct(
                entitySymbol,
                entityCustomizationScheme,
                dbContextScheme);

            var getByIdQueryScheme = new CrudGeneratorScheme<CqrsOperationWithoutReturnValueWithReturnValueGeneratorConfiguration>(
                entityScheme,
                dbContextScheme,
                GetByIdQueryDefaultConfigurationFactory.Construct(globalConfiguration, sharedConfiguration).Build(entityScheme.EntityName));
            var generateGetByIdQuery = new GetByIdQueryCrudGenerator(
                context,
                getByIdQueryScheme);
            generateGetByIdQuery.RunGenerator();
            if (generateGetByIdQuery.EndpointMap is not null)
            {
                endpointsMaps.Add(generateGetByIdQuery.EndpointMap);
            }
            
            var getListQueryScheme = new CrudGeneratorScheme<CqrsListOperationWithoutReturnValueGeneratorConfiguration>(
                entityScheme,
                dbContextScheme,
                GetListQueryDefaultConfigurationFactory.Construct(globalConfiguration, sharedConfiguration).Build(entityScheme.EntityName));
            var generateListQuery = new ListQueryCrudGenerator(
                context,
                getListQueryScheme);
            generateListQuery.RunGenerator();
            if (generateListQuery.EndpointMap is not null)
            {
                endpointsMaps.Add(generateListQuery.EndpointMap);
            }
            
            var createCommandScheme = new CrudGeneratorScheme<CqrsOperationWithoutReturnValueWithReturnValueGeneratorConfiguration>(
                entityScheme,
                dbContextScheme,
                CreateCommandDefaultConfigurationFactory.Construct(globalConfiguration, sharedConfiguration).Build(entityScheme.EntityName));
            var generateCreateCommand = new CreateCommandCrudGenerator(
                context,
                createCommandScheme,
                getByIdQueryScheme.Configuration.Endpoint.RouteConfiguration);
            generateCreateCommand.RunGenerator();
            if (generateCreateCommand.EndpointMap is not null)
            {
                endpointsMaps.Add(generateCreateCommand.EndpointMap);
            }

            var updateCommandScheme = new CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration>(
                entityScheme,
                dbContextScheme,
                UpdateCommandDefaultConfigurationFactory.Construct(globalConfiguration, sharedConfiguration).Build(entityScheme.EntityName));
            var generateUpdateCommand = new UpdateCommandCrudGenerator(
                context,
                updateCommandScheme);
            generateUpdateCommand.RunGenerator();
            if (generateUpdateCommand.EndpointMap is not null)
            {
                endpointsMaps.Add(generateUpdateCommand.EndpointMap);
            }

            var deleteCommandScheme = new CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration>(
                entityScheme,
                dbContextScheme,
                DeleteCommandDefaultConfigurationFactory.Construct(globalConfiguration, sharedConfiguration).Build(entityScheme.EntityName));
            var generateDeleteCommand = new DeleteCommandCrudGenerator(
                context,
                deleteCommandScheme);
            generateDeleteCommand.RunGenerator();
            if (generateDeleteCommand.EndpointMap is not null)
            {
                endpointsMaps.Add(generateDeleteCommand.EndpointMap);
            }
        }

        var mapEndpointsGenerator = new MapEndpointsGenerator(context, endpointsMaps, globalConfiguration);
        mapEndpointsGenerator.RunGenerator();
    }
}

internal class MapEndpointsGenerator : BaseGenerator
{
    private readonly List<EndpointMap> _endpointsMaps;
    private readonly GlobalCqrsGeneratorConfiguration _globalConfiguration;
    private readonly string _endpointMapsClassName;

    public MapEndpointsGenerator(
        GeneratorExecutionContext context,
        List<EndpointMap> endpointsMaps,
        GlobalCqrsGeneratorConfiguration globalConfiguration)
        : base(context,
            globalConfiguration.AutogeneratedFileText,
            globalConfiguration.NullableEnable)
    {
        _endpointsMaps = endpointsMaps;
        _globalConfiguration = globalConfiguration;
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
            Usings = string.Join("", usings),
            PutIntoNamespace = "Mars.Api",
            ExtensionClassName = _endpointMapsClassName,
            Maps = string.Join("", maps)
        };
        WriteFile(
            $"{_globalConfiguration.TemplatesBasePath}.GeneratedEndpointsMapExtension.txt",
            model,
            _endpointMapsClassName);
    }
}

internal class CrudGeneratorScheme<TConfiguration> where TConfiguration : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public EntityScheme EntityScheme { get; set; }
    public DbContextScheme DbContextScheme { get; set; }
    public TConfiguration Configuration { get; set; }

    public CrudGeneratorScheme(EntityScheme entityScheme, DbContextScheme dbContextScheme, TConfiguration configuration)
    {
        EntityScheme = entityScheme;
        DbContextScheme = dbContextScheme;
        Configuration = configuration;
    }
}