using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ITech.CrudGenerator.Tests.Endpoints.Core;

internal class ApiFactory : WebApplicationFactory<Program>
{
    private readonly IConfiguration _configuration;

    public ApiFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string BaseApiPath => "http://localhost/";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var inMemoryConfiguration = new Dictionary<string, string>
        {
            { "ConnectionStrings:DefaultConnection", _configuration.GetConnectionString("DefaultConnection")! }
        };

        // see https://github.com/dotnet/aspnetcore/issues/37680#issuecomment-1235651426
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfiguration!)
            .Build();

        builder.UseConfiguration(config);
    }
}