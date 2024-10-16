using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.UserFeature.CreateUser;
using ITech.CrudGenerator.TestApi.Application.UserFeature.GetUser;
using ITech.CrudGenerator.TestApi.Application.UserFeature.GetUsers;
using ITech.CrudGenerator.TestApi.Application.UserFeature.UpdateUser;
using ITech.CrudGenerator.TestApi.Generators.UserGenerator;
using ITech.CrudGenerator.Tests.Endpoints.Core;

namespace ITech.CrudGenerator.Tests.Endpoints;

[Collection("E2eTests")]
public class UserEndpointTests(TestApiFixture fixture)
{
    private readonly TestMongoDb _db = fixture.GetDb();
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    [Theory]
    [InlineData("user/{0}")]
    public async Task Should_GetUser(string endpoint)
    {
        // Arrange
        var user = await CreateUserAsync();

        // Act
        var response = await _httpClient.GetAsync(string.Format(endpoint, user.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<UserDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(user.Id);
        actual.Name.Should().Be("Test User");
    }
    
    [Theory]
    [InlineData("user?page=1&pageSize=10")]
    public async Task Should_GetUsersList(string endpoint)
    {
        // Arrange
        await CreateUserAsync();

        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<UsersDto>();
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
    [InlineData("user/create")]
    public async Task Should_CreateUser(string endpoint)
    {
        // Act
        var response =
            await _httpClient.PostAsJsonAsync(endpoint, new CreateUserCommand { Name = "New User" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<CreatedUserDto>();
        actual.Should().NotBeNull();
        actual!.Id.Should().NotBeEmpty();

        // Assert saved to db
        var user = await _db.FindAsync<User>([actual.Id], new CancellationToken());
        user.Should().NotBeNull();
        user!.Name.Should().Be("New User");
    }
    
    [Theory]
    [InlineData("user/{0}/update")]
    public async Task Should_UpdateUser(string endpoint)
    {
        // Arrange
        var createdUser = await CreateUserAsync();

        // Act
        var response = await _httpClient.PutAsJsonAsync(
            string.Format(endpoint, createdUser.Id),
            new UpdateUserCommand(createdUser.Id) { Name = "Updated user name" });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var user = await _db.FindAsync<User>([createdUser.Id], new CancellationToken());
        user.Should().NotBeNull();
        user!.Name.Should().Be("Updated user name");
    }
    
    [Theory]
    [InlineData("user/{0}/delete")]
    public async Task Should_DeleteUser(string endpoint)
    {
        // Arrange
        var createdUser = await CreateUserAsync();

        // Act
        var response = await _httpClient.DeleteAsync(string.Format(endpoint, createdUser.Id));
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert deleted from db
        var user = await _db.FindAsync<User>([createdUser.Id], new CancellationToken());
        user.Should().BeNull();
    }


    private async Task<User> CreateUserAsync()
    {
        var user = new User { Id = Guid.NewGuid(), Name = "Test User" };
        await _db.AddAsync(user);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        return user;
    }
}