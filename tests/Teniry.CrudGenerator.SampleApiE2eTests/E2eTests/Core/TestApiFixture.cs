using Teniry.CrudGenerator.SampleApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Teniry.Cqrs.ApplicationEvents.EventsChannelHandler;

namespace Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

public class TestApiFixture : IAsyncLifetime {
    private readonly ApiFactory _apiFactory;
    private readonly IConfigurationRoot _configuration;

    public TestApiFixture() {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName!)
            .AddJsonFile("appsettings.tests.json", false)
            .AddEnvironmentVariables()
            .AddUserSecrets(typeof(ApiFactory).Assembly, true)
            .Build();

        _apiFactory = new(_configuration);
    }

    public async Task InitializeAsync() {
        var db = GetDb();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        await DbDataInitializer.InitializeAsync(db);
    }

    public async Task DisposeAsync() {
        await _apiFactory.DisposeAsync();
    }

    public HttpClient GetHttpClient() {
        var httpClient = _apiFactory.CreateClient();
        httpClient.BaseAddress = new(_apiFactory.BaseApiPath);

        return httpClient;
    }

    // Db контекст должен создаваться для каждого теста новый
    // потому что, ef core кэширует полученные данные если не был вызван AsNoTracking
    // и этот кэш может привести к тому, что в одном тесте была сделана выборка, результат закэшировался
    // при его вызове в следующем тесте, ef возьмет закэшированный результат и тест не выполнится
    public SampleMongoDb GetDb() {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var connectionStringDbName = _configuration.GetConnectionString("DefaultConnectionDbName");

        var optionsBuilder = new DbContextOptionsBuilder<SampleMongoDb>()
            .UseMongoDB(connectionString!, connectionStringDbName!)
            .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddDebug()));

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(it => it.GetService(typeof(EventsChannel)))
            .Returns(new EventsChannel());

        var db = new SampleMongoDb(optionsBuilder.Options, serviceProvider.Object);
        // TODO: decide on Transactional behaviour
        // https://devblogs.microsoft.com/dotnet/mongodb-ef-core-provider-whats-new/#autotransactions-and-optimistic-concurrency
        // Transactions added to ef provider, but mongo should be configured additionally,
        // System.NotSupportedException
        // The MongoDB EF Core Provider now uses transactions to ensure all updates in a SaveChanges
        // operation are applied together or not at all. Your current MongoDB server configuration
        // does not support transactions and you should consider switching to a replica set
        // or load balanced configuration. If you are sure you do not need save consistency
        // or optimistic concurrency you can disable transactions by setting
        // 'Database.AutoTransactionBehavior = AutoTransactionBehavior.Never' on your DbContext.
        // To fix need to configure mongo
        db.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;

        return db;
    }
}