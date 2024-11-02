using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.SimpleEntityFeature.CreateSimpleEntity;
using ITech.CrudGenerator.TestApi.Application.SimpleEntityFeature.GetSimpleEntities;
using ITech.CrudGenerator.TestApi.Application.SimpleEntityFeature.GetSimpleEntity;
using ITech.CrudGenerator.TestApi.Application.SimpleEntityFeature.UpdateSimpleEntity;
using ITech.CrudGenerator.TestApi.Generators.SimpleEntityGenerator;
using ITech.CrudGenerator.Tests.Endpoints.Core;

namespace ITech.CrudGenerator.Tests.Endpoints.SimpleEntitiesTests;

[Collection("E2eTests")]
public class SimpleEntityEndpointTests(TestApiFixture fixture)
{
    private readonly TestMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("simpleEntity/{0}")]
    public async Task Should_GetSimpleEntity(string endpoint)
    {
        // Arrange
        var createdEntity = await CreateSimpleEntityAsync("Entity to get");

        // Act
        var response = await _httpClient.GetAsync(string.Format(endpoint, createdEntity.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<SimpleEntityDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(createdEntity.Id);
        actual.Name.Should().Be("Entity to get");
    }

    [Theory]
    [InlineData("simpleEntity?page=1&pageSize=10")]
    public async Task Should_GetSimpleEntitiesList(string endpoint)
    {
        // Arrange
        await CreateSimpleEntityAsync("Entity to get one of list");

        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<SimpleEntitiesDto>();
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
    [InlineData("simpleEntity/create")]
    public async Task Should_CreateSimpleEntity(string endpoint)
    {
        // Act
        var response =
            await _httpClient.PostAsJsonAsync(endpoint, new CreateSimpleEntityCommand { Name = "My new entity" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<CreatedSimpleEntityDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().NotBeEmpty();

        // Assert saved to db
        var entity = await _db.FindAsync<SimpleEntity>([actual.Id], new CancellationToken());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("My new entity");
    }

    [Theory]
    [InlineData("simpleEntity/{0}/update")]
    public async Task Should_UpdateSimpleEntity(string endpoint)
    {
        // Arrange
        var createdEntity = await CreateSimpleEntityAsync("Entity to update");

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            string.Format(endpoint, createdEntity.Id),
            new UpdateSimpleEntityCommand(createdEntity.Id) { Name = "Updated entity name" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var entity = await _db.FindAsync<SimpleEntity>([createdEntity.Id], new CancellationToken());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("Updated entity name");
    }

    [Theory]
    [InlineData("simpleEntity/{0}/delete")]
    public async Task Should_DeleteSimpleEntity(string endpoint)
    {
        // Arrange
        var createdSimpleEntity = await CreateSimpleEntityAsync("Entity to delete");

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(endpoint, createdSimpleEntity.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert deleted from db
        var entity = await _db.FindAsync<SimpleEntity>([createdSimpleEntity.Id], new CancellationToken());
        entity.Should().BeNull();
    }

    private async Task<SimpleEntity> CreateSimpleEntityAsync(string name)
    {
        var entity = new SimpleEntity { Id = Guid.NewGuid(), Name = name };
        await _db.AddAsync(entity);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        return entity;
    }
}