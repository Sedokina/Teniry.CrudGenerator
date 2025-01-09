using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Crud;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;
using ITech.CrudGenerator.CrudGeneratorCore.Schemes.Entity;

namespace ITech.CrudGenerator.CrudGeneratorCore.OperationsGenerators.Core;

internal abstract class BaseOperationCrudGenerator<TConfiguration> : BaseGenerator
    where TConfiguration : CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    protected readonly CrudGeneratorScheme<TConfiguration> Scheme;
    protected readonly EntityScheme EntityScheme;
    public EndpointMap? EndpointMap { get; set; }

    protected BaseOperationCrudGenerator(CrudGeneratorScheme<TConfiguration> scheme)
        : base(scheme.Configuration.GlobalConfiguration.AutogeneratedFileText,
            scheme.Configuration.GlobalConfiguration.NullableEnable)
    {
        Scheme = scheme;
        EntityScheme = scheme.EntityScheme;
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

internal class EndpointMap
{
    public string EntityTitle { get; set; }
    public string EndpointNamespace { get; set; }
    public string HttpMethod { get; }
    public string EndpointRoute { get; set; }
    public string ClassName { get; set; }
    public string FunctionName { get; set; }

    public EndpointMap(string entityTitle, string endpointNamespace, string httpMethod, string endpointRoute,
        string className, string functionName)
    {
        EntityTitle = entityTitle;
        EndpointNamespace = endpointNamespace;
        HttpMethod = httpMethod;
        EndpointRoute = endpointRoute;
        ClassName = className;
        FunctionName = functionName;
    }
}