using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.CreateCompany;
using ITech.CrudGenerator.TestApi.Generators.CompanyGenerator;
using Microsoft.EntityFrameworkCore;

namespace ITech.CrudGenerator.Tests.Endpoints;

[Collection("E2eTests")]
public class CompanyEndpointTests
{
    private readonly TestMongoDb _db;
    private readonly TestApiFixture _fixture;
    private readonly HttpClient _httpClient;

    public CompanyEndpointTests(TestApiFixture fixture)
    {
        _fixture = fixture;
        _httpClient = _fixture.GetHttpClient();
        _db = _fixture.GetDb();
    }


    [Theory]
    [InlineData("company/create")]
    public async Task Should_CreateAcceptanceDocument(string endpoint)
    {
        // Act
        var response =
            await _httpClient.PostAsJsonAsync(endpoint, new CreateCompanyCommand { Name = "My new company" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        var actual = await response.Content.ReadFromJsonAsync<CreatedCompanyDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().NotBeEmpty();

        // Assert saved to db
        var company = await _db.FindAsync<Company>([actual.Id], new CancellationToken());
        var a = await _db.Set<Company>().ToListAsync();
        company.Should().NotBeNull();
        company!.Name.Should().Be("My new company");
    }
}