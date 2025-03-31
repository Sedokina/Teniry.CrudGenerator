using System.Net;
using System.Net.Http.Json;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.CustomManagedEntityFeature.ManagedEntityCreateOperationCustomNs;
using Teniry.CrudGenerator.SampleApi.Application.CustomManagedEntityFeature.ManagedEntityUpdateOperationCustomNs;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomManagedEntityGenerator;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.CustomEntitiesTests;

[Collection("E2eTests")]
public class CustomManagedEntityEndpointTests(TestApiFixture fixture) {
    private readonly SampleMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("customizedManagedEntityCreate")]
    public async Task Should_CreateEntity(string endpoint) {
        // Act
        var response = await _httpClient
            .PostAsJsonAsync(endpoint, new CustomizedNameCreateManagedEntityCommand { Name = "My new entity" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<CustomizedNameCreatedManagedEntityDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().NotBeEmpty();

        // Assert get route returned
        response.Headers.Location.Should().BeNull("because get endpoint is not generated for this entity");

        // Assert saved to db
        var entity = await _db.FindAsync<CustomManagedEntity>([actual.Id], new());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("My new entity");
    }

    [Theory]
    [InlineData("customizedManagedEntityUpdate/{0}")]
    public async Task Should_UpdateEntity(string endpoint) {
        // Arrange
        var createdEntity = await CreateEntityAsync("Entity to update");

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            string.Format(endpoint, createdEntity.Id),
            new CustomizedNameUpdateManagedEntityCommand(createdEntity.Id) { Name = "Updated entity name" }
        );
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var entity = await _db.FindAsync<CustomManagedEntity>([createdEntity.Id], new());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("Updated entity name");
    }

    [Theory]
    [InlineData("customizedManagedEntityDelete/customManagedEntity/{0}")]
    public async Task Should_DeleteEntity(string endpoint) {
        // Arrange
        var createdSimpleEntity = await CreateEntityAsync("Entity to delete");

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(endpoint, createdSimpleEntity.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert deleted from db
        var entity = await _db.FindAsync<CustomManagedEntity>([createdSimpleEntity.Id], new());
        entity.Should().BeNull();
    }

    [Theory]
    [InlineData("customManagedEntity?page=1&pageSize=10")]
    [InlineData("customManagedEntity/691cd56c-46ee-4151-ae10-029a25e32d1b")]
    public async Task Should_NotGenerateGetEndpoints(string endpoint) {
        // Act
        var response = await _httpClient.SendAsync(new(HttpMethod.Options, endpoint));

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<CustomManagedEntity> CreateEntityAsync(string name) {
        var entity = new CustomManagedEntity { Id = Guid.NewGuid(), Name = name };
        await _db.AddAsync(entity);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();

        return entity;
    }
}