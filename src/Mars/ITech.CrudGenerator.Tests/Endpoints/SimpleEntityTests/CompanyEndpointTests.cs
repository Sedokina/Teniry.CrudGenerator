using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.CreateCompany;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.GetCompanies;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.GetCompany;
using ITech.CrudGenerator.TestApi.Application.CompanyFeature.UpdateCompany;
using ITech.CrudGenerator.TestApi.Generators.CompanyGenerator;
using ITech.CrudGenerator.Tests.Endpoints.Core;

namespace ITech.CrudGenerator.Tests.Endpoints.SimpleEntityTests;

[Collection("E2eTests")]
public class CompanyEndpointTests(TestApiFixture fixture)
{
    private readonly TestMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("company/{0}")]
    public async Task Should_GetCompany(string endpoint)
    {
        // Arrange
        var createdCompany = await CreateCompanyAsync("Company to get");

        // Act
        var response = await _httpClient.GetAsync(string.Format(endpoint, createdCompany.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<CompanyDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(createdCompany.Id);
        actual.Name.Should().Be("Company to get");
    }

    [Theory]
    [InlineData("company?page=1&pageSize=10")]
    public async Task Should_GetCompaniesList(string endpoint)
    {
        // Arrange
        await CreateCompanyAsync("Company to get one of list");

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
    [InlineData("company/{0}/update")]
    public async Task Should_UpdateCompany(string endpoint)
    {
        // Arrange
        var createdCompany = await CreateCompanyAsync("Company to update");

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            string.Format(endpoint, createdCompany.Id),
            new UpdateCompanyCommand(createdCompany.Id) { Name = "Updated company name" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var company = await _db.FindAsync<Company>([createdCompany.Id], new CancellationToken());
        company.Should().NotBeNull();
        company!.Name.Should().Be("Updated company name");
    }

    [Theory]
    [InlineData("company/{0}/delete")]
    public async Task Should_DeleteCompany(string endpoint)
    {
        // Arrange
        var createdCompany = await CreateCompanyAsync("Company to delete");

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(endpoint, createdCompany.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert deleted from db
        var company = await _db.FindAsync<Company>([createdCompany.Id], new CancellationToken());
        company.Should().BeNull();
    }

    private async Task<Company> CreateCompanyAsync(string companyName)
    {
        var company = new Company { Id = Guid.NewGuid(), Name = companyName };
        await _db.AddAsync(company);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        return company;
    }
}