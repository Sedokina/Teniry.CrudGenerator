using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.SimpleTypeEntityFeature.CreateSimpleTypeEntity;
using ITech.CrudGenerator.TestApi.Application.SimpleTypeEntityFeature.GetSimpleTypeEntities;
using ITech.CrudGenerator.TestApi.Application.SimpleTypeEntityFeature.GetSimpleTypeEntity;
using ITech.CrudGenerator.TestApi.Application.SimpleTypeEntityFeature.UpdateSimpleTypeEntity;
using ITech.CrudGenerator.TestApi.Generators.SimpleTypeEntityGenerator;
using ITech.CrudGenerator.Tests.Endpoints.Core;

namespace ITech.CrudGenerator.Tests.Endpoints.SimpleEntitiesTests;

[Collection("E2eTests")]
public class SimpleTypeEntityEndpointTests(TestApiFixture fixture)
{
    private readonly TestMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("simpleTypeEntity/{0}")]
    public async Task Should_GetSimpleTypeEntity(string endpoint)
    {
        // Arrange
        var entity = await CreateSimpleTypeEntityAsync();

        // Act
        var response = await _httpClient.GetAsync(string.Format(endpoint, entity.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<SimpleTypeEntityDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(entity.Id);
        actual.Name.Should().Be("Test User");
        actual.Code.Should().Be('a');
        actual.IsActive.Should().Be(true);
        actual.RegistrationDate.Should().NotBeBefore(DateTime.Today.Date.ToUniversalTime());
        actual.LastSignInDate.Should().NotBeBefore(DateTime.Today.Date.ToUniversalTime());
        actual.ByteRating.Should().BeGreaterThan(0);
        actual.ShortRating.Should().BeLessThan(0);
        actual.IntRating.Should().BeLessThan(0);
        actual.LongRating.Should().BeLessThan(0);
        actual.SByteRating.Should().BeLessThan(0);
        actual.UShortRating.Should().BeGreaterThan(0);
        actual.UIntRating.Should().BeGreaterThan(0);
        actual.ULongRating.Should().BeGreaterThan(0);
        actual.FloatRating.Should().BeGreaterThan(0);
        actual.DoubleRating.Should().BeGreaterThan(0);
        actual.DecimalRating.Should().BeGreaterThan(0);
        actual.NotIdGuid.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("simpleTypeEntity?page=1&pageSize=10")]
    public async Task Should_GetSimpleTypeEntitiesList(string endpoint)
    {
        // Arrange
        await CreateSimpleTypeEntityAsync();

        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<SimpleTypeEntitiesDto>();
        actual.Should().NotBeNull();
        actual!.Page.PageSize.Should().BeGreaterThan(0);
        actual.Page.CurrentPageIndex.Should().BeGreaterThan(0);
        actual.Items.Should().HaveCountGreaterThanOrEqualTo(1);
        actual.Items
            .Where(x => x.Name.Equals("Test User"))
            .Should().AllSatisfy(x =>
            {
                x.Id.Should().NotBeEmpty();
                x.Name.Should().Be("Test User");
                x.Code.Should().Be('a');
                x.IsActive.Should().Be(true);
                x.RegistrationDate.Should().NotBeBefore(DateTime.Today.Date.ToUniversalTime());
                x.LastSignInDate.Should().NotBeBefore(DateTime.Today.Date.ToUniversalTime());
                x.ByteRating.Should().BeGreaterThan(0);
                x.ShortRating.Should().BeLessThan(0);
                x.IntRating.Should().BeLessThan(0);
                x.LongRating.Should().BeLessThan(0);
                x.SByteRating.Should().BeLessThan(0);
                x.UShortRating.Should().BeGreaterThan(0);
                x.UIntRating.Should().BeGreaterThan(0);
                x.ULongRating.Should().BeGreaterThan(0);
                x.FloatRating.Should().BeGreaterThan(0);
                x.DoubleRating.Should().BeGreaterThan(0);
                x.DecimalRating.Should().BeGreaterThan(0);
                x.NotIdGuid.Should().NotBeEmpty();
            });
    }

    [Theory]
    [InlineData("simpleTypeEntity/create")]
    public async Task Should_CreateSimpleTypeEntity(string endpoint)
    {
        // Act
        var response =
            await _httpClient.PostAsJsonAsync(endpoint, new CreateSimpleTypeEntityCommand { Name = "New User" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<CreatedSimpleTypeEntityDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().NotBeEmpty();

        // Assert saved to db
        var entity = await _db.FindAsync<SimpleTypeEntity>([actual.Id], new CancellationToken());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("New User");
    }

    [Theory]
    [InlineData("simpleTypeEntity/{0}/update")]
    public async Task Should_UpdateSimpleTypeEntity(string endpoint)
    {
        // Arrange
        var createdEntity = await CreateSimpleTypeEntityAsync();

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            string.Format(endpoint, createdEntity.Id),
            new UpdateSimpleTypeEntityCommand(createdEntity.Id)
            {
                Name = "Updated user name",
                Code = 'b',
                IsActive = false,
                RegistrationDate = DateTime.UtcNow.AddDays(-1),
                LastSignInDate = DateTimeOffset.UtcNow.AddDays(-1),
                ByteRating = 13,
                ShortRating = -95,
                IntRating = -873198873,
                LongRating = -17263715389164,
                SByteRating = -9,
                UShortRating = 95,
                UIntRating = 873198873,
                ULongRating = 17263715389164,
                FloatRating = 28.54f,
                DoubleRating = 87189.86378,
                DecimalRating = 9813.7641635291m,
                NotIdGuid = new Guid("8e358827-0a9d-4d02-9d07-a7265a76b5ae"),
            });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var entity = await _db.FindAsync<SimpleTypeEntity>([createdEntity.Id], new CancellationToken());
        entity.Should().NotBeNull();
        entity!.Name.Should().Be("Updated user name");
        entity.Code.Should().Be('b');
        entity.IsActive.Should().Be(false);
        entity.RegistrationDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(-1), TimeSpan.FromMinutes(5));
        entity.LastSignInDate.Should().BeCloseTo(DateTimeOffset.UtcNow.AddDays(-1), TimeSpan.FromMinutes(5));
        entity.ByteRating.Should().Be(13);
        entity.ShortRating.Should().Be(-95);
        entity.IntRating.Should().Be(-873198873);
        entity.LongRating.Should().Be(-17263715389164);
        entity.SByteRating.Should().Be(-9);
        entity.UShortRating.Should().Be(95);
        entity.UIntRating.Should().Be(873198873);
        entity.ULongRating.Should().Be(17263715389164);
        entity.FloatRating.Should().BeApproximately(28.54f, 0.01f);
        entity.DoubleRating.Should().BeApproximately(87189.86378, 0.01f);
        entity.DecimalRating.Should().BeApproximately(9813.7641635291m, 0.01m);
        entity.NotIdGuid.Should().Be(new Guid("8e358827-0a9d-4d02-9d07-a7265a76b5ae"));
    }

    [Theory]
    [InlineData("simpleTypeEntity/{0}/delete")]
    public async Task Should_DeleteSimpleTypeEntity(string endpoint)
    {
        // Arrange
        var createdEntity = await CreateSimpleTypeEntityAsync();

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(endpoint, createdEntity.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert deleted from db
        var entity = await _db.FindAsync<SimpleTypeEntity>([createdEntity.Id], new CancellationToken());
        entity.Should().BeNull();
    }


    private async Task<SimpleTypeEntity> CreateSimpleTypeEntityAsync()
    {
        var entity = new SimpleTypeEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Code = 'a',
            IsActive = true,
            RegistrationDate = DateTime.Today,
            LastSignInDate = DateTimeOffset.Now,
            ByteRating = 1,
            ShortRating = -83,
            IntRating = -19876718,
            LongRating = -971652637891,
            SByteRating = -4,
            UShortRating = 83,
            UIntRating = 19876718,
            ULongRating = 971652637891,
            FloatRating = 18.13f,
            DoubleRating = 91873.862378,
            DecimalRating = 867.97716829m,
            NotIdGuid = new Guid("63c4e04c-77d3-4e27-b490-8f6e4fc635bd"),
        };
        await _db.AddAsync(entity);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        return entity;
    }
}