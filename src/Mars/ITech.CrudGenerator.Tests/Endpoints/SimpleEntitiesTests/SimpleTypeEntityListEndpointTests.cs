using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using ITech.CrudGenerator.TestApi.Application.SimpleTypeEntityFeature.GetSimpleTypeEntities;
using ITech.CrudGenerator.Tests.Endpoints.Core;

namespace ITech.CrudGenerator.Tests.Endpoints.SimpleEntitiesTests;

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
}