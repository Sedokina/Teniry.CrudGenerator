using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.IntIdEntityFeature.CreateIntIdEntity;
using ITech.CrudGenerator.TestApi.Application.IntIdEntityFeature.GetIntIdEntities;
using ITech.CrudGenerator.TestApi.Application.IntIdEntityFeature.GetIntIdEntity;
using ITech.CrudGenerator.TestApi.Application.IntIdEntityFeature.UpdateIntIdEntity;
using ITech.CrudGenerator.TestApi.Generators.IntIdEntityGenerator;
using ITech.CrudGenerator.Tests.E2eTests.Core;

namespace ITech.CrudGenerator.Tests.E2eTests.SimpleEntitiesTests;

[Collection("E2eTests")]
public class IntIdEntityEndpointTests(TestApiFixture fixture)
{
    private readonly TestMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("intIdEntity/{0}")]
    public async Task Should_GetEntity(string endpoint)
    {
        // Arrange
        var createdEntity = await CreateEntityAsync("Entity to get");

        // Act
        var response = await _httpClient.GetAsync(string.Format(endpoint, createdEntity.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<IntIdEntityDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(createdEntity.Id);
        actual.Name.Should().Be("Entity to get");
    }
    
    [Theory]
    [InlineData("intIdEntity?page=1&pageSize=10")]
    public async Task Should_GetEntitiesList(string endpoint)
    {
        // Arrange
        await CreateEntityAsync("Entity to get one of list");

        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<IntIdEntitiesDto>();
        actual.Should().NotBeNull();
        actual!.Page.PageSize.Should().BeGreaterThan(0);
        actual.Page.CurrentPageIndex.Should().BeGreaterThan(0);
        actual.Items.Should().HaveCountGreaterThanOrEqualTo(1);
        actual.Items.Should().AllSatisfy(x =>
        {
            x.Id.Should().BeGreaterThan(0);
            x.Name.Should().NotBeNullOrEmpty();
        });
    }
    
    [Theory]
    [InlineData("intIdEntity/create")]
    public async Task Should_CreateEntity(string endpoint)
    {
        // Act
        var response =
            await _httpClient.PostAsJsonAsync(endpoint, new CreateIntIdEntityCommand { Name = "My new entity" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<CreatedIntIdEntityDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().BeGreaterThan(0);

        // Assert saved to db
        var entity = await _db.FindAsync<IntIdEntity>([actual.Id], new CancellationToken());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("My new entity");
    }
    
    [Theory]
    [InlineData("intIdEntity/{0}/update")]
    public async Task Should_UpdateEntity(string endpoint)
    {
        // Arrange
        var createdEntity = await CreateEntityAsync("Entity to update");

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            string.Format(endpoint, createdEntity.Id),
            new UpdateIntIdEntityCommand(createdEntity.Id) { Name = "Updated entity name" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var entity = await _db.FindAsync<IntIdEntity>([createdEntity.Id], new CancellationToken());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("Updated entity name");
    }

    [Theory]
    [InlineData("intIdEntity/{0}/delete")]
    public async Task Should_DeleteEntity(string endpoint)
    {
        // Arrange
        var createdIntIdEntity = await CreateEntityAsync("Entity to delete");

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(endpoint, createdIntIdEntity.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert deleted from db
        var entity = await _db.FindAsync<IntIdEntity>([createdIntIdEntity.Id], new CancellationToken());
        entity.Should().BeNull();
    }

    private async Task<IntIdEntity> CreateEntityAsync(string name)
    {
        var entity = new IntIdEntity { Name = name };
        await _db.AddAsync(entity);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        return entity;
    }
}