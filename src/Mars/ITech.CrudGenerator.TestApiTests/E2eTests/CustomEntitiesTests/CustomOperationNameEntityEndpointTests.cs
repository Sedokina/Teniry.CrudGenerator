using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomOperationNameEntityFeature.CustomOpCreateCustomOperationNameEntity;
using ITech.CrudGenerator.TestApi.Application.CustomOperationNameEntityFeature.CustomOpGetByIdCustomOperationNameEntity;
using ITech.CrudGenerator.TestApi.Application.CustomOperationNameEntityFeature.
    CustomOpGetListCustomOperationNameEntities;
using ITech.CrudGenerator.TestApi.Application.CustomOperationNameEntityFeature.CustomOpUpdateCustomOperationNameEntity;
using ITech.CrudGenerator.TestApi.Generators.CustomOperationNameEntityGenerator;
using ITech.CrudGenerator.TestApiTests.E2eTests.Core;

namespace ITech.CrudGenerator.TestApiTests.E2eTests.CustomEntitiesTests;

[Collection("E2eTests")]
public class CustomOperationNameEntityEndpointTests(TestApiFixture fixture) {
    private readonly TestMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("customOperationNameEntity/{0}")]
    public async Task Should_GetEntity(string endpoint) {
        // Arrange
        var createdEntity = await CreateEntityAsync("Entity to get");

        // Act
        var response = await _httpClient.GetAsync(string.Format(endpoint, createdEntity.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<CustomOperationNameEntityDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(createdEntity.Id);
        actual.Name.Should().Be("Entity to get");
    }

    [Theory]
    [InlineData("customOperationNameEntity?page=1&pageSize=10")]
    public async Task Should_GetEntitiesList(string endpoint) {
        // Arrange
        await CreateEntityAsync("Entity to get one of list");

        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<CustomOperationNameEntitiesDto>();
        actual.Should().NotBeNull();
        actual!.Page.PageSize.Should().BeGreaterThan(0);
        actual.Page.CurrentPageIndex.Should().BeGreaterThan(0);
        actual.Items.Should().HaveCountGreaterThanOrEqualTo(1);
        actual.Items.Should().AllSatisfy(
            x => {
                x.Id.Should().NotBeEmpty();
                x.Name.Should().NotBeNullOrEmpty();
            }
        );
    }

    [Theory]
    [InlineData("customOperationNameEntity/customOpCreate")]
    public async Task Should_CreateEntity(string endpoint) {
        // Act
        var response = await _httpClient
            .PostAsJsonAsync(endpoint, new CustomOpCreateCustomOperationNameEntityCommand { Name = "My new entity" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<CreatedCustomOperationNameEntityDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().NotBeEmpty();

        // Assert saved to db
        var entity = await _db.FindAsync<CustomOperationNameEntity>([actual.Id], new());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("My new entity");
    }

    [Theory]
    [InlineData("customOperationNameEntity/{0}/customOpUpdate")]
    public async Task Should_UpdateEntity(string endpoint) {
        // Arrange
        var createdEntity = await CreateEntityAsync("Entity to update");

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            string.Format(endpoint, createdEntity.Id),
            new CustomOpUpdateCustomOperationNameEntityCommand(createdEntity.Id) { Name = "Updated entity name" }
        );
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var entity = await _db.FindAsync<CustomOperationNameEntity>([createdEntity.Id], new());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("Updated entity name");
    }

    [Theory]
    [InlineData("customOperationNameEntity/{0}/customOpDelete")]
    public async Task Should_DeleteEntity(string endpoint) {
        // Arrange
        var createdEntity = await CreateEntityAsync("Entity to delete");

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(endpoint, createdEntity.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert deleted from db
        var entity = await _db.FindAsync<CustomOperationNameEntity>([createdEntity.Id], new());
        entity.Should().BeNull();
    }

    private async Task<CustomOperationNameEntity> CreateEntityAsync(string name) {
        var entity = new CustomOperationNameEntity { Id = Guid.NewGuid(), Name = name };
        await _db.AddAsync(entity);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();

        return entity;
    }
}