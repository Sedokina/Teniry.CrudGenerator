using ITech.Cqrs.Cqrs.ApplicationEvents.EventsChannelHandler;
using ITech.CrudGenerator.TestApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ITech.CrudGenerator.Tests.Endpoints;

public class TestApiFixture
{
    private readonly ApiFactory _apiFactory;
    private readonly IConfigurationRoot _configuration;

    public TestApiFixture()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName!)
            .AddJsonFile("appsettings.tests.json", false)
            .AddEnvironmentVariables()
            .AddUserSecrets(typeof(ApiFactory).Assembly, true)
            .Build();

        _apiFactory = new(_configuration);
    }

    public HttpClient GetHttpClient()
    {
        var httpClient = _apiFactory.CreateClient();
        httpClient.BaseAddress = new(_apiFactory.BaseApiPath);

        return httpClient;
    }


    // Db контекст должен создаваться для каждого теста новый
    // потому что, ef core кэширует полученные данные если не был вызван AsNoTracking
    // и этот кэш может привести к тому, что в одном тесте была сделана выборка, результат закэшировался
    // при его вызове в следующем тесте, ef возьмет закэшированный результат и тест не выполнится
    public TestMongoDb GetDb()
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<TestMongoDb>().UseMongoDB(connectionString!, "MarsDb")
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()));

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(it => it.GetService(typeof(EventsChannel)))
            .Returns(new EventsChannel());

        return new(optionsBuilder.Options, serviceProvider.Object);
    }
}