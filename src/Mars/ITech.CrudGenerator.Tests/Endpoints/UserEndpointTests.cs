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
        actual.NotId.Should().NotBeEmpty();
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
            x.NotId.Should().NotBeEmpty();
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
            new UpdateUserCommand(createdUser.Id)
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
                NotId = new Guid("8e358827-0a9d-4d02-9d07-a7265a76b5ae"),
            });
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Assert saved to db
        var user = await _db.FindAsync<User>([createdUser.Id], new CancellationToken());
        user.Should().NotBeNull();
        user!.Name.Should().Be("Updated user name");
        user.Code.Should().Be('b');
        user.IsActive.Should().Be(false);
        user.RegistrationDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(-1), TimeSpan.FromMinutes(5));
        user.LastSignInDate.Should().BeCloseTo(DateTimeOffset.UtcNow.AddDays(-1), TimeSpan.FromMinutes(5));
        user.ByteRating.Should().Be(13);
        user.ShortRating.Should().Be(-95);
        user.IntRating.Should().Be(-873198873);
        user.LongRating.Should().Be(-17263715389164);
        user.SByteRating.Should().Be(-9);
        user.UShortRating.Should().Be(95);
        user.UIntRating.Should().Be(873198873);
        user.ULongRating.Should().Be(17263715389164);
        user.FloatRating.Should().BeApproximately(28.54f, 0.01f);
        user.DoubleRating.Should().BeApproximately(87189.86378, 0.01f);
        user.DecimalRating.Should().BeApproximately(9813.7641635291m, 0.01m);
        user.NotId.Should().Be(new Guid("8e358827-0a9d-4d02-9d07-a7265a76b5ae"));
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
        var user = new User
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
            NotId = new Guid("63c4e04c-77d3-4e27-b490-8f6e4fc635bd"),
        };
        await _db.AddAsync(user);
        await _db.SaveChangesAsync();
        _db.ChangeTracker.Clear();
        return user;
    }
}