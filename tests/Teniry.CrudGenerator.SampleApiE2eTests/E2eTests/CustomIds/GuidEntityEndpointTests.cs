using System.Net;
using System.Net.Http.Json;
using Teniry.Cqrs.Extended.Types.PatchOperationType;
using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.GuidEntityFeature.CreateGuidEntity;
using Teniry.CrudGenerator.SampleApi.Application.GuidEntityFeature.GetGuidEntities;
using Teniry.CrudGenerator.SampleApi.Application.GuidEntityFeature.GetGuidEntity;
using Teniry.CrudGenerator.SampleApi.Application.GuidEntityFeature.PatchGuidEntity;
using Teniry.CrudGenerator.SampleApi.Application.GuidEntityFeature.UpdateGuidEntity;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomIds.GuidEntityGenerator;
using Teniry.CrudGenerator.SampleApi.Endpoints.GuidEntityEndpoints;
using Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

namespace Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.CustomIds;

[Collection("E2eTests")]
public class GuidEntitiesEndpointTests(TestApiFixture fixture) {
    private readonly SampleMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("guidEntity/{0}")]
    public async Task Should_GetEntity(string endpoint) {
        // Arrange
        var createdEntity = await CreateEntityAsync("Entity to get");

        // Act
        var response = await _httpClient.GetAsync(string.Format(endpoint, createdEntity.GuidEntityId));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<GuidEntityDto>();
        actual.Should().NotBeNull();
        actual!.GuidEntityId.Should().Be(createdEntity.GuidEntityId);
        actual.Name.Should().Be("Entity to get");
    }

    [Theory]
    [InlineData("guidEntity?page=1&pageSize=10")]
    public async Task Should_GetEntitiesList(string endpoint) {
        // Arrange
        await CreateEntityAsync("Entity to get one of list");

        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<GuidEntitiesDto>();
        actual.Should().NotBeNull();
        actual!.Page.PageSize.Should().BeGreaterThan(0);
        actual.Page.CurrentPageIndex.Should().BeGreaterThan(0);
        actual.Items.Should().HaveCountGreaterThanOrEqualTo(1);
        actual.Items.Should().AllSatisfy(
            x => {
                x.GuidEntityId.Should().NotBeEmpty();
                x.Name.Should().NotBeNullOrEmpty();
            }
        );
    }

    [Theory]
    [InlineData("guidEntity/create")]
    public async Task Should_CreateEntity(string endpoint) {
        // Act
        var response =
            await _httpClient.PostAsJsonAsync(endpoint, new CreateGuidEntityCommand { Name = "My new entity" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<CreatedGuidEntityDto>();
        actual.Should().NotBeNull();
        actual!.GuidEntityId.Should().NotBeEmpty();

        // Assert get route returned
        response.Headers.Location.Should().NotBeNull()
            .And.Subject.ToString().Should().NotBeNullOrEmpty();

        // Assert saved to db
        var entity = await _db.FindAsync<GuidEntity>([actual.GuidEntityId], new());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("My new entity");
    }

    [Theory]
    [InlineData("guidEntity/{0}/update")]
    public async Task Should_UpdateEntity(string endpoint) {
        // Arrange
        var createdEntity = await CreateEntityAsync("Entity to update");

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            string.Format(endpoint, createdEntity.GuidEntityId),
            new UpdateGuidEntityVm { Name = "Updated entity name" }
        );
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var entity = await _db.FindAsync<GuidEntity>([createdEntity.GuidEntityId], new());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("Updated entity name");
    }

    [Theory]
    [InlineData("guidEntity/{0}/patch")]
    public async Task Should_PatchEntity(string endpoint) {
        // Arrange
        var createdEntity = await CreateEntityAsync("Entity to patch");

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            string.Format(endpoint, createdEntity.GuidEntityId),
            new PatchGuidEntityVm { Name = new("Patched entity name", PatchOpType.Update) }
        );
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var entity = await _db.FindAsync<GuidEntity>([createdEntity.GuidEntityId], new());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("Patched entity name");
    }

    [Theory]
    [InlineData("guidEntity/{0}/delete")]
    public async Task Should_DeleteEntity(string endpoint) {
        // Arrange
        var createdGuidEntity = await CreateEntityAsync("Entity to delete");

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(endpoint, createdGuidEntity.GuidEntityId));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert deleted from db
        var entity = await _db.FindAsync<GuidEntity>([createdGuidEntity.GuidEntityId], new());
        entity.Should().BeNull();
    }

    private async Task<GuidEntity> CreateEntityAsync(string name) {
        var entity = new GuidEntity { GuidEntityId = Guid.NewGuid(), Name = name };
        await _db.AddAsync(entity);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();

        return entity;
    }
}