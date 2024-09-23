using System;
using System.Collections.Generic;
using System.Linq;
using Mars.Generators.CrudGeneratorCore.Configurations.Global;
using Mars.Generators.CrudGeneratorCore.Configurations.Global.Factories;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using Mars.Generators.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using Mars.Generators.CrudGeneratorCore.ConfigurationsReceiver;
using Mars.Generators.CrudGeneratorCore.OperationsGenerators;
using Mars.Generators.CrudGeneratorCore.OperationsGenerators.Core;
using Mars.Generators.CrudGeneratorCore.Schemes.DbContext;
using Mars.Generators.CrudGeneratorCore.Schemes.Entity;
using Mars.Generators.CrudGeneratorCore.Schemes.EntityCustomization;
using Microsoft.CodeAnalysis;

namespace Mars.Generators;

[Generator]
public class CrudGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new EntityGeneratorConfigurationSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            if (context.SyntaxReceiver is not EntityGeneratorConfigurationSyntaxReceiver syntaxReceiver) return;

            var dbContextScheme = DbContextSchemeFactory.Construct(context);

            List<EndpointMap> endpointsMaps = new();
            var globalConfigurationBuilder = GlobalCrudGeneratorConfigurationDefaultConfigurationFactory.Construct();
            var sharedConfigurationBuilder = CqrsOperationsSharedConfigurationBuilderFactory.Construct();

            foreach (var classSyntax in syntaxReceiver.ClassesForCrudGeneration)
            {
                var (entityGeneratorConfigurationSymbol, entitySymbol) = classSyntax.AsSymbol(context);

                var entityCustomizationScheme = EntityCastomizationSchemeFactory
                    .Construct(entityGeneratorConfigurationSymbol as INamedTypeSymbol, context);
                var entityScheme = EntitySchemeFactory.Construct(
                    entitySymbol,
                    entityCustomizationScheme,
                    dbContextScheme);

                var getByIdQueryConfigurationBuilder = GetByIdQueryDefaultConfigurationBuilderFactory
                    .Construct(globalConfigurationBuilder, sharedConfigurationBuilder);
                var getByIdQueryScheme = new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
                    entityScheme,
                    dbContextScheme,
                    getByIdQueryConfigurationBuilder.Build(entityScheme));
                var generateGetByIdQuery = new GetByIdQueryCrudGenerator(
                    context,
                    getByIdQueryScheme);
                generateGetByIdQuery.RunGenerator();
                if (generateGetByIdQuery.EndpointMap is not null)
                {
                    endpointsMaps.Add(generateGetByIdQuery.EndpointMap);
                }

                var getListQueryScheme = new CrudGeneratorScheme<CqrsListOperationGeneratorConfiguration>(
                    entityScheme,
                    dbContextScheme,
                    GetListQueryDefaultConfigurationBulderFactory
                        .Construct(globalConfigurationBuilder, sharedConfigurationBuilder)
                        .Build(entityScheme));
                var generateListQuery = new ListQueryCrudGenerator(
                    context,
                    getListQueryScheme);
                generateListQuery.RunGenerator();
                if (generateListQuery.EndpointMap is not null)
                {
                    endpointsMaps.Add(generateListQuery.EndpointMap);
                }

                var createCommandConfiguration = CreateCommandDefaultConfigurationBuilderFactory
                    .Construct(
                        globalConfigurationBuilder,
                        sharedConfigurationBuilder,
                        entityCustomizationScheme.CreateOperation)
                    .Build(entityScheme);
                if (createCommandConfiguration.Generate)
                {
                    var createCommandScheme =
                        new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
                            entityScheme,
                            dbContextScheme,
                            createCommandConfiguration
                        );
                    var generateCreateCommand = new CreateCommandCrudGenerator(
                        context,
                        createCommandScheme,
                        getByIdQueryConfigurationBuilder.Endpoint.RouteConfigurationBuilder,
                        getByIdQueryConfigurationBuilder.OperationName);
                    generateCreateCommand.RunGenerator();
                    if (generateCreateCommand.EndpointMap is not null)
                    {
                        endpointsMaps.Add(generateCreateCommand.EndpointMap);
                    }
                }

                var updateCommandScheme =
                    new CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration>(
                        entityScheme,
                        dbContextScheme,
                        UpdateCommandDefaultConfigurationBuilderFactory
                            .Construct(globalConfigurationBuilder, sharedConfigurationBuilder)
                            .Build(entityScheme));
                var generateUpdateCommand = new UpdateCommandCrudGenerator(
                    context,
                    updateCommandScheme);
                generateUpdateCommand.RunGenerator();
                if (generateUpdateCommand.EndpointMap is not null)
                {
                    endpointsMaps.Add(generateUpdateCommand.EndpointMap);
                }

                var deleteCommandScheme =
                    new CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration>(
                        entityScheme,
                        dbContextScheme,
                        DeleteCommandDefaultConfigurationBuilderFactory
                            .Construct(
                                globalConfigurationBuilder, 
                                sharedConfigurationBuilder,
                                entityCustomizationScheme.DeleteOperation)
                            .Build(entityScheme));
                var generateDeleteCommand = new DeleteCommandCrudGenerator(
                    context,
                    deleteCommandScheme);
                generateDeleteCommand.RunGenerator();
                if (generateDeleteCommand.EndpointMap is not null)
                {
                    endpointsMaps.Add(generateDeleteCommand.EndpointMap);
                }
            }

            var mapEndpointsGenerator = new MapEndpointsGenerator(context, endpointsMaps, globalConfigurationBuilder);
            mapEndpointsGenerator.RunGenerator();
        }
        catch (Exception e)
        {
            Logger.Print($"{e.Message}\n{e.StackTrace}");
            Logger.FlushLogs(context);
        }
    }
}

internal class MapEndpointsGenerator : BaseGenerator
{
    private readonly List<EndpointMap> _endpointsMaps;
    private readonly GlobalCqrsGeneratorConfigurationBuilder _globalConfiguration;
    private readonly string _endpointMapsClassName;

    public MapEndpointsGenerator(
        GeneratorExecutionContext context,
        List<EndpointMap> endpointsMaps,
        GlobalCqrsGeneratorConfigurationBuilder globalConfiguration)
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

internal class CrudGeneratorScheme<TConfiguration>
    where TConfiguration : CqrsOperationWithoutReturnValueGeneratorConfiguration
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