using System.Collections.Generic;
using System.Linq;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global.Factories;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.Builders;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuildersFactories;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators;
using ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.InternalEntityGenerator;
using ITech.CrudGenerator.Extensions;
using Microsoft.CodeAnalysis;

namespace ITech.CrudGenerator;

[Generator]
public sealed class CrudGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // var diagnostics = context.CompilationProvider.Select((compilation, token) => Diagnostic.Create());
        // context.RegisterSourceOutput(diagnostics, static (context, diagnostic) => context.ReportDiagnostic(diagnostic));

        var generatorConfigurationsProviders = context.SyntaxProvider.CreateGeneratorConfigurationsProvider();
        var dbContextSchemeProviders = context.SyntaxProvider.CreateDbContextConfigurationsProvider();

        List<EndpointMap> endpointsMaps = [];
        var globalConfigurationBuilder = GlobalCrudGeneratorConfigurationDefaultConfigurationFactory.Construct();
        var sharedConfigurationBuilder = new CqrsOperationsSharedConfigurationBuilderFactory().Construct();

        var generatorConfigurationsWithDbContextProviders =
            generatorConfigurationsProviders.Combine(dbContextSchemeProviders.Collect())
                .WithTrackingName("generatorConfigurationsWithDbContextProviders");

        IConfigurationBuilderFactory[] factories =
        [
            new GetByIdQueryDefaultConfigurationBuilderFactory(),
            new GetListQueryDefaultConfigurationBulderFactory(),
            new CreateCommandDefaultConfigurationBuilderFactory(),
            new UpdateCommandDefaultConfigurationBuilderFactory(),
            new DeleteCommandDefaultConfigurationBuilderFactory()
        ];
        var entitySchemeFactoryWithDbContextProviders = generatorConfigurationsWithDbContextProviders
            .Select((tuple, token) => (tuple.Left, new EntitySchemeFactory().Construct(
                    tuple.Left,
                    tuple.Right[0]), tuple.Right[0])
            ).WithTrackingName("entitySchemeFactoryWithDbContextProviders");
        var temp = entitySchemeFactoryWithDbContextProviders.SelectMany((tuple, token) =>
        {
            var result = new object[]
            {
                new GetByIdQueryDefaultConfigurationBuilderFactory().Construct(globalConfigurationBuilder,
                    sharedConfigurationBuilder,
                    tuple.Left.GetByIdOperation
                ),
                new GetListQueryDefaultConfigurationBulderFactory().Construct(globalConfigurationBuilder,
                    sharedConfigurationBuilder,
                    tuple.Left.GetListOperation
                ),
                new CreateCommandDefaultConfigurationBuilderFactory().Construct(globalConfigurationBuilder,
                    sharedConfigurationBuilder,
                    tuple.Left.CreateOperation
                ),
                new UpdateCommandDefaultConfigurationBuilderFactory().Construct(globalConfigurationBuilder,
                    sharedConfigurationBuilder,
                    tuple.Left.UpdateOperation
                ),
                new DeleteCommandDefaultConfigurationBuilderFactory().Construct(globalConfigurationBuilder,
                    sharedConfigurationBuilder,
                    tuple.Left.DeleteOperation
                )
            };
            return result;
        });

        // TODO: check if there are dbContextSchemes at all
        // TODO: add errors log

        context.RegisterSourceOutput(entitySchemeFactoryWithDbContextProviders,
            (productionContext, generatorConfigurationWithDbContext) =>
                Execute(
                    productionContext,
                    generatorConfigurationWithDbContext.Item1,
                    generatorConfigurationWithDbContext.Item2,
                    generatorConfigurationWithDbContext.Item3,
                    globalConfigurationBuilder,
                    sharedConfigurationBuilder,
                    endpointsMaps
                )
        );

        context.RegisterSourceOutput(dbContextSchemeProviders.Collect(), (productionContext, _) =>
        {
            var mapEndpointsGenerator = new EndpointsMapGenerator(endpointsMaps, globalConfigurationBuilder);
            mapEndpointsGenerator.RunGenerator();
            WriteFiles(productionContext, mapEndpointsGenerator.GeneratedFiles);
        });
    }

    private static void Execute(
        SourceProductionContext context,
        InternalEntityGeneratorConfiguration internalEntityGeneratorConfiguration,
        EntityScheme entityScheme,
        DbContextScheme dbContextScheme,
        GlobalCqrsGeneratorConfigurationBuilder globalConfigurationBuilder,
        CqrsOperationsSharedConfigurationBuilder sharedConfigurationBuilder,
        List<EndpointMap> endpointsMaps)
    {
        var getByIdQueryConfigurationBuilder = new GetByIdQueryDefaultConfigurationBuilderFactory()
            .Construct(
                globalConfigurationBuilder,
                sharedConfigurationBuilder,
                internalEntityGeneratorConfiguration.GetByIdOperation);

        var getByIdQueryConfiguration = getByIdQueryConfigurationBuilder.Build(entityScheme);
        // if (getByIdQueryConfiguration.Generate)
        // {
        //     var getByIdQueryScheme =
        //         new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
        //             entityScheme,
        //             dbContextScheme,
        //             getByIdQueryConfiguration);
        //     var generateGetByIdQuery = new GetByIdQueryCrudGenerator(getByIdQueryScheme);
        //     generateGetByIdQuery.RunGenerator();
        //     WriteFiles(context, generateGetByIdQuery.GeneratedFiles);
        //     if (generateGetByIdQuery.EndpointMap is not null)
        //     {
        //         endpointsMaps.Add(generateGetByIdQuery.EndpointMap);
        //     }
        // }

        // var getListConfiguration = new GetListQueryDefaultConfigurationBulderFactory()
        //     .Construct(
        //         globalConfigurationBuilder,
        //         sharedConfigurationBuilder,
        //         internalEntityGeneratorConfiguration.GetListOperation)
        //     .Build(entityScheme);
        // if (getListConfiguration.Generate)
        // {
        //     var getListQueryScheme = new CrudGeneratorScheme<CqrsListOperationGeneratorConfiguration>(
        //         entityScheme,
        //         dbContextScheme,
        //         getListConfiguration);
        //     var generateListQuery = new ListQueryCrudGenerator(getListQueryScheme);
        //     generateListQuery.RunGenerator();
        //     WriteFiles(context, generateListQuery.GeneratedFiles);
        //     if (generateListQuery.EndpointMap is not null)
        //     {
        //         endpointsMaps.Add(generateListQuery.EndpointMap);
        //     }
        // }

        // var createCommandConfiguration = new CreateCommandDefaultConfigurationBuilderFactory()
        //     .Construct(
        //         globalConfigurationBuilder,
        //         sharedConfigurationBuilder,
        //         internalEntityGeneratorConfiguration.CreateOperation)
        //     .Build(entityScheme);
        // if (createCommandConfiguration.Generate)
        // {
        //     var createCommandScheme =
        //         new CrudGeneratorScheme<CqrsOperationWithReturnValueGeneratorConfiguration>(
        //             entityScheme,
        //             dbContextScheme,
        //             createCommandConfiguration
        //         );
        //     var generateCreateCommand = new CreateCommandCrudGenerator(
        //         createCommandScheme,
        //         getByIdQueryConfiguration.Endpoint.Generate
        //             ? getByIdQueryConfigurationBuilder.Endpoint.RouteConfigurationBuilder
        //             : null,
        //         getByIdQueryConfiguration.Endpoint.Generate
        //             ? getByIdQueryConfigurationBuilder.OperationName
        //             : null);
        //     generateCreateCommand.RunGenerator();
        //     WriteFiles(context, generateCreateCommand.GeneratedFiles);
        //     if (generateCreateCommand.EndpointMap is not null)
        //     {
        //         endpointsMaps.Add(generateCreateCommand.EndpointMap);
        //     }
        // }

        // var updateOperationConfiguration = new UpdateCommandDefaultConfigurationBuilderFactory()
        //     .Construct(
        //         globalConfigurationBuilder,
        //         sharedConfigurationBuilder,
        //         internalEntityGeneratorConfiguration.UpdateOperation)
        //     .Build(entityScheme);
        // if (updateOperationConfiguration.Generate)
        // {
        //     var updateCommandScheme =
        //         new CrudGeneratorScheme<CqrsOperationWithReturnValueWithReceiveViewModelGeneratorConfiguration>(
        //             entityScheme,
        //             dbContextScheme,
        //             updateOperationConfiguration);
        //     var generateUpdateCommand = new UpdateCommandCrudGenerator(updateCommandScheme);
        //     generateUpdateCommand.RunGenerator();
        //     WriteFiles(context, generateUpdateCommand.GeneratedFiles);
        //     if (generateUpdateCommand.EndpointMap is not null)
        //     {
        //         endpointsMaps.Add(generateUpdateCommand.EndpointMap);
        //     }
        // }

        var deleteCommandConfiguration = new DeleteCommandDefaultConfigurationBuilderFactory()
            .Construct(
                globalConfigurationBuilder,
                sharedConfigurationBuilder,
                internalEntityGeneratorConfiguration.DeleteOperation)
            .Build(entityScheme);
        if (deleteCommandConfiguration.Generate)
        {
            var deleteCommandScheme =
                new CrudGeneratorScheme<CqrsOperationWithoutReturnValueGeneratorConfiguration>(
                    entityScheme,
                    dbContextScheme,
                    deleteCommandConfiguration);
            var generateDeleteCommand = new DeleteCommandCrudGenerator(deleteCommandScheme);
            generateDeleteCommand.RunGenerator();
            WriteFiles(context, generateDeleteCommand.GeneratedFiles);
            if (generateDeleteCommand.EndpointMap is not null)
            {
                endpointsMaps.Add(generateDeleteCommand.EndpointMap);
            }
        }
    }

    private static void WriteFiles(SourceProductionContext context, List<GeneratorResult> files)
    {
        foreach (var file in files)
        {
            context.AddSource(file.FileName, file.Source);
        }
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