using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.CreateCompany;
using ITech.CrudGenerator.TestApi.Generators.CompanyGenerator;

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
    public async Task Should_CreateCompany(string endpoint)
    {
        // Act
        var response =
            await _httpClient.PostAsJsonAsync(endpoint, new CreateCompanyCommand { Name = "My new company" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<CreatedCompanyDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().NotBeEmpty();

        // Assert saved to db
        var company = await _db.FindAsync<Company>([actual.Id], new CancellationToken());
        company.Should().NotBeNull();
        company!.Name.Should().Be("My new company");
    }
    
    [Theory]
    [InlineData("company/a8b57bd8-81c6-446c-9cc4-39356ac3bd3d/delete")]
    public async Task Should_DeleteCompany(string endpoint)
    {
        // Arrange
        var id = new Guid("a8b57bd8-81c6-446c-9cc4-39356ac3bd3d");
        await _db.AddAsync(new Company { Id = id, Name = "Company to delete" });
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();

        // Act
        var response = await _httpClient.DeleteAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Assert deleted from db
        var company = await _db.FindAsync<Company>([id], new CancellationToken());
        company.Should().BeNull();
    }
}