using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi.Application.SimpleTypeEntityFeature.GetSimpleTypeEntities;
using ITech.CrudGenerator.TestApiTests.E2eTests.Core;

namespace ITech.CrudGenerator.TestApiTests.E2eTests.SimpleEntitiesTests;

[Collection("E2eTests")]
public class SimpleTypeEntityListEndpointTests(TestApiFixture fixture)
{
    private readonly HttpClient _httpClient = fixture.GetHttpClient();

    public static TheoryData<string, string, Expression<Func<SimpleTypeEntitiesListItemDto, object>>> SortData => new()
    {
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.name", "asc", x => x.Name },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.name", "desc", x => x.Name },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.code", "asc", x => x.Code },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.code", "desc", x => x.Code },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.isActive", "asc", x => x.IsActive },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.isActive", "desc", x => x.IsActive },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.registrationDate", "asc", x => x.RegistrationDate },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.registrationDate", "desc", x => x.RegistrationDate },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.lastSignInDate", "asc", x => x.LastSignInDate },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.lastSignInDate", "desc", x => x.LastSignInDate },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.byteRating", "asc", x => x.ByteRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.byteRating", "desc", x => x.ByteRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.shortRating", "asc", x => x.ShortRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.shortRating", "desc", x => x.ShortRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.intRating", "asc", x => x.IntRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.intRating", "desc", x => x.IntRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.longRating", "asc", x => x.LongRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.longRating", "desc", x => x.LongRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.sByteRating", "asc", x => x.SByteRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.sByteRating", "desc", x => x.SByteRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.uShortRating", "asc", x => x.UShortRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.uShortRating", "desc", x => x.UShortRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.uIntRating", "asc", x => x.UIntRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.uIntRating", "desc", x => x.UIntRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.uLongRating", "asc", x => x.ULongRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.uLongRating", "desc", x => x.ULongRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.floatRating", "asc", x => x.FloatRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.floatRating", "desc", x => x.FloatRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.doubleRating", "asc", x => x.DoubleRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.doubleRating", "desc", x => x.DoubleRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=asc.decimalRating", "asc", x => x.DecimalRating },
        { "simpleTypeEntity?page=1&pageSize=10&sort=desc.decimalRating", "desc", x => x.DecimalRating }
    };

    [Theory]
    [MemberData(nameof(SortData))]
    public async Task Should_SortListResult(
        string endpoint,
        string direction,
        Expression<Func<SimpleTypeEntitiesListItemDto, object>> property)
    {
        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<SimpleTypeEntitiesDto>();

        if (direction.Equals("asc"))
        {
            actual!.Items.Should().BeInAscendingOrder(property);
        }
        else
        {
            actual!.Items.Should().BeInDescendingOrder(property);
        }
    }

    public static TheoryData<string, Expression<Func<SimpleTypeEntitiesListItemDto, bool>>> FilterData => new()
    {
        {
            "simpleTypeEntity?page=1&pageSize=10&id=44bacea2-1e32-452a-b1f3-28e46924e899",
            x => x.Id == new Guid("44bacea2-1e32-452a-b1f3-28e46924e899")
        },
        { "simpleTypeEntity?page=1&pageSize=10&name=First", x => x.Name.Contains("First") },
        { "simpleTypeEntity?page=1&pageSize=10&code=a", x => x.Code == 'a' },
        {
            $"simpleTypeEntity?page=1&pageSize=10&registrationDateFrom={DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture)}",
            x => x.RegistrationDate.Date >= DateTime.Now.Date
        },
        {
            $"simpleTypeEntity?page=1&pageSize=10&registrationDateTo={DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture)}",
            x => x.RegistrationDate < DateTime.Now.Date
        },
        {
            $"simpleTypeEntity?page=1&pageSize=10&lastSignInDateFrom={DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture)}",
            x => x.LastSignInDate >= DateTime.UtcNow.AddMinutes(-5)
        },
        {
            $"simpleTypeEntity?page=1&pageSize=10&lastSignInDateTo={DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture)}",
            x => x.LastSignInDate < DateTime.UtcNow
        },
        { "simpleTypeEntity?page=1&pageSize=10&isActive=true", x => x.IsActive == true },
        { "simpleTypeEntity?page=1&pageSize=10&byteRatingFrom=1", x => x.ByteRating >= 1 },
        { "simpleTypeEntity?page=1&pageSize=10&byteRatingTo=2", x => x.ByteRating < 2 },
        { "simpleTypeEntity?page=1&pageSize=10&shortRatingFrom=-84", x => x.ShortRating >= -84 },
        { "simpleTypeEntity?page=1&pageSize=10&shortRatingTo=-83", x => x.ShortRating < -83 },
        { "simpleTypeEntity?page=1&pageSize=10&intRatingFrom=-19876720", x => x.IntRating >= -19876720 },
        { "simpleTypeEntity?page=1&pageSize=10&intRatingTo=-19876718", x => x.IntRating < -19876718 },
        { "simpleTypeEntity?page=1&pageSize=10&longRatingFrom=-971652637899", x => x.LongRating >= -971652637899 },
        { "simpleTypeEntity?page=1&pageSize=10&longRatingTo=-971652637891", x => x.LongRating < -971652637891 },
        { "simpleTypeEntity?page=1&pageSize=10&sByteRatingFrom=-5", x => x.SByteRating >= -5 },
        { "simpleTypeEntity?page=1&pageSize=10&sByteRatingTo=-4", x => x.SByteRating < -4 },
        { "simpleTypeEntity?page=1&pageSize=10&uShortRatingFrom=84", x => x.UShortRating >= 84 },
        { "simpleTypeEntity?page=1&pageSize=10&uShortRatingTo=100", x => x.UShortRating < 100 },
        { "simpleTypeEntity?page=1&pageSize=10&uIntRatingFrom=19876720", x => x.UIntRating >= 19876720 },
        { "simpleTypeEntity?page=1&pageSize=10&uIntRatingTo=20876718", x => x.UIntRating < 20876718 },
        { "simpleTypeEntity?page=1&pageSize=10&uLongRatingFrom=971652637899", x => x.ULongRating >= 971652637899 },
        { "simpleTypeEntity?page=1&pageSize=10&uLongRatingTo=999652637891", x => x.ULongRating < 999652637891 },
        { "simpleTypeEntity?page=1&pageSize=10&floatRatingFrom=19.13", x => x.FloatRating >= 18.13f },
        { "simpleTypeEntity?page=1&pageSize=10&floatRatingTo=20.13", x => x.FloatRating < 20.13f },
        { "simpleTypeEntity?page=1&pageSize=10&doubleRatingFrom=91880.862378", x => x.DoubleRating >= 91880.862378 },
        { "simpleTypeEntity?page=1&pageSize=10&doubleRatingTo=99873.862378", x => x.DoubleRating < 99873.862378 },
        { "simpleTypeEntity?page=1&pageSize=10&decimalRatingFrom=869.97716829", x => x.DecimalRating >= 869.97716829m },
        { "simpleTypeEntity?page=1&pageSize=10&decimalRatingTo=967.97716829", x => x.DecimalRating < 967.97716829m },
        {
            "simpleTypeEntity?page=1&pageSize=10&notIdGuid=f6c5e2d1-b438-4faf-8521-b775d783f6f3",
            x => x.NotIdGuid == new Guid("f6c5e2d1-b438-4faf-8521-b775d783f6f3")
        }
    };

    [Theory]
    [MemberData(nameof(FilterData))]
    public async Task Should_FilterResult(string endpoint,
        Expression<Func<SimpleTypeEntitiesListItemDto, bool>> validation)
    {
        // Act
        var response = await _httpClient.GetAsync(endpoint);
        response.Should().FailIfNotSuccessful();

        // Assert correct response
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<SimpleTypeEntitiesDto>();
        actual!.Items.Should().HaveCountGreaterThan(0).And.OnlyContain(validation);
    }
}