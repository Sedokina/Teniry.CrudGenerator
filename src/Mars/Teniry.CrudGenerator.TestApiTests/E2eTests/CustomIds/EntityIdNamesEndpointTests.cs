using System.Net;
using System.Net.Http.Json;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.EntityIdNameFeature.CreateEntityIdName;
using Teniry.CrudGenerator.SampleApi.Application.EntityIdNameFeature.GetEntityIdName;
using Teniry.CrudGenerator.SampleApi.Application.EntityIdNameFeature.GetEntityIdNames;
using Teniry.CrudGenerator.SampleApi.Application.EntityIdNameFeature.UpdateEntityIdName;
using Teniry.CrudGenerator.SampleApi.Generators.CustomIds.EntityIdNameGenerator;
using Teniry.CrudGenerator.TestApiTests.E2eTests.Core;

namespace Teniry.CrudGenerator.TestApiTests.E2eTests.CustomIds;

[Collection("E2eTests")]
public class EntityIdNamesEndpointTests(TestApiFixture fixture) {
    private readonly TestMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("entityIdName/{0}")]
    public async Task Should_GetEntity(string endpoint) {
        // Arrange
        var createdEntity = await CreateEntityAsync("Entity to get");

        // Act
        var response = await _httpClient.GetAsync(string.Format(endpoint, createdEntity.EntityIdNameId));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<EntityIdNameDto>();
        actual.Should().NotBeNull();
        actual!.EntityIdNameId.Should().Be(createdEntity.EntityIdNameId);
        actual.Name.Should().Be("Entity to get");
    }

    [Theory]
    [InlineData("entityIdName?page=1&pageSize=10")]
    public async Task Should_GetEntitiesList(string endpoint) {
        // Arrange
        await CreateEntityAsync("Entity to get one of list");

        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<EntityIdNamesDto>();
        actual.Should().NotBeNull();
        actual!.Page.PageSize.Should().BeGreaterThan(0);
        actual.Page.CurrentPageIndex.Should().BeGreaterThan(0);
        actual.Items.Should().HaveCountGreaterThanOrEqualTo(1);
        actual.Items.Should().AllSatisfy(
            x => {
                x.EntityIdNameId.Should().NotBeEmpty();
                x.Name.Should().NotBeNullOrEmpty();
            }
        );
    }

    [Theory]
    [InlineData("entityIdName/create")]
    public async Task Should_CreateEntity(string endpoint) {
        // Act
        var response =
            await _httpClient.PostAsJsonAsync(endpoint, new CreateEntityIdNameCommand { Name = "My new entity" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<CreatedEntityIdNameDto>();
        actual.Should().NotBeNull();
        actual!.EntityIdNameId.Should().NotBeEmpty();

        // Assert get route returned
        response.Headers.Location.Should().NotBeNull()
            .And.Subject.ToString().Should().NotBeNullOrEmpty();

        // Assert saved to db
        var entity = await _db.FindAsync<EntityIdName>([actual.EntityIdNameId], new());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("My new entity");
    }

    [Theory]
    [InlineData("entityIdName/{0}/update")]
    public async Task Should_UpdateEntity(string endpoint) {
        // Arrange
        var createdEntity = await CreateEntityAsync("Entity to update");

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            string.Format(endpoint, createdEntity.EntityIdNameId),
            new UpdateEntityIdNameCommand(createdEntity.EntityIdNameId) { Name = "Updated entity name" }
        );
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var entity = await _db.FindAsync<EntityIdName>([createdEntity.EntityIdNameId], new());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("Updated entity name");
    }

    [Theory]
    [InlineData("entityIdName/{0}/delete")]
    public async Task Should_DeleteEntity(string endpoint) {
        // Arrange
        var createdEntityIdName = await CreateEntityAsync("Entity to delete");

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(endpoint, createdEntityIdName.EntityIdNameId));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert deleted from db
        var entity = await _db.FindAsync<EntityIdName>([createdEntityIdName.EntityIdNameId], new());
        entity.Should().BeNull();
    }

    private async Task<EntityIdName> CreateEntityAsync(string name) {
        var entity = new EntityIdName { EntityIdNameId = Guid.NewGuid(), Name = name };
        await _db.AddAsync(entity);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();

        return entity;
    }
}