using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.CreateCompany;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.GetCompanies;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.GetCompany;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.UpdateCompany;
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
    [InlineData("company/1dc25f7c-feb4-42e5-a1e0-2df2ee96be83")]
    public async Task Should_GetCompany(string endpoint)
    {
        // Arrange
        var id = new Guid("1dc25f7c-feb4-42e5-a1e0-2df2ee96be83");
        await _db.AddAsync(new Company { Id = id, Name = "Company to get" });
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        
        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var actual = await response.Content.ReadFromJsonAsync<CompanyDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(id);
        actual.Name.Should().Be("Company to get");
    }
    
    [Theory]
    [InlineData("company?page=1&pageSize=10")]
    public async Task Should_GetCompaniesList(string endpoint)
    {
        // Arrange
        var id = new Guid("ac637119-dcc9-4144-8d7a-38632d9fce27");
        await _db.AddAsync(new Company { Id = id, Name = "Company to get one of list" });
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        
        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var actual = await response.Content.ReadFromJsonAsync<CompaniesDto>();
        actual.Should().NotBeNull();
        actual!.Page.PageSize.Should().BeGreaterThan(0);
        actual.Page.CurrentPageIndex.Should().BeGreaterThan(0);
        actual.Items.Should().HaveCountGreaterThanOrEqualTo(1);
        actual.Items.Should().AllSatisfy(x =>
        {
            x.Id.Should().NotBeEmpty();
            x.Name.Should().NotBeNullOrEmpty();
        });
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
    [InlineData("company/90355ca9-d101-46a7-a694-a4c5a04b5015/update")]
    public async Task Should_UpdateCompany(string endpoint)
    {
        // Arrange
        var id = new Guid("90355ca9-d101-46a7-a694-a4c5a04b5015");
        await _db.AddAsync(new Company { Id = id, Name = "Company to update" });
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        
        // Act
        var response =
            await _httpClient.PutAsJsonAsync(endpoint, new UpdateCompanyCommand(id) { Name = "Updated company name" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var company = await _db.FindAsync<Company>([id], new CancellationToken());
        company.Should().NotBeNull();
        company!.Name.Should().Be("Updated company name");
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