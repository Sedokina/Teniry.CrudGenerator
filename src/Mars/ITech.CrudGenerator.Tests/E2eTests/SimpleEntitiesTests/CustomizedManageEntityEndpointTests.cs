using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomizedManageEntityFeature.CreateCustomizedManageEntity;
using ITech.CrudGenerator.TestApi.Application.CustomizedManageEntityFeature.UpdateCustomizedManageEntity;
using ITech.CrudGenerator.TestApi.Generators.CustomizedManageEntity;
using ITech.CrudGenerator.Tests.E2eTests.Core;

namespace ITech.CrudGenerator.Tests.E2eTests.SimpleEntitiesTests;

[Collection("E2eTests")]
public class CustomizedManageEntityEndpointTests(TestApiFixture fixture)
{
    private readonly TestMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("customizedManageEntityCreate")]
    public async Task Should_CreateCustomizedEntity(string endpoint)
    {
        // Act
        var response = await _httpClient
            .PostAsJsonAsync(endpoint, new CustomizedNameCreateManageEntityCommand { Name = "My new entity" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<CustomizedNameCreatedManageEntityDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().NotBeEmpty();

        // Assert saved to db
        var entity = await _db.FindAsync<CustomizedManageEntity>([actual.Id], new CancellationToken());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("My new entity");
    }

    [Theory]
    [InlineData("customizedManageEntityUpdate/{0}")]
    public async Task Should_UpdateCustomizedEntity(string endpoint)
    {
        // Arrange
        var createdEntity = await CreateSimpleEntityAsync("Entity to update");

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            string.Format(endpoint, createdEntity.Id),
            new CustomizedNameUpdateManageEntityCommand(createdEntity.Id) { Name = "Updated entity name" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var entity = await _db.FindAsync<CustomizedManageEntity>([createdEntity.Id], new CancellationToken());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("Updated entity name");
    }

    [Theory]
    [InlineData("customizedManageEntityDelete/customizedManageEntity/{0}")]
    public async Task Should_DeleteCustomizedEntity(string endpoint)
    {
        // Arrange
        var createdSimpleEntity = await CreateSimpleEntityAsync("Entity to delete");

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(endpoint, createdSimpleEntity.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert deleted from db
        var entity = await _db.FindAsync<CustomizedManageEntity>([createdSimpleEntity.Id], new CancellationToken());
        entity.Should().BeNull();
    }
    
    [Theory]
    [InlineData("customizedmanageentity?page=1&pageSize=10")]
    [InlineData("customizedmanageentity/691cd56c-46ee-4151-ae10-029a25e32d1b")]
    public async Task Should_NotGenerateGetEndpoints(string endpoint)
    {
        // Act
        var response = await _httpClient.GetAsync(endpoint);

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    
    private async Task<CustomizedManageEntity> CreateSimpleEntityAsync(string name)
    {
        var entity = new CustomizedManageEntity { Id = Guid.NewGuid(), Name = name };
        await _db.AddAsync(entity);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        return entity;
    }
}