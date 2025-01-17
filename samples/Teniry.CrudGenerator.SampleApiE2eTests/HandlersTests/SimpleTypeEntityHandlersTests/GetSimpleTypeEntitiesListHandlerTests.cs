using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.SimpleTypeEntityFeature.GetSimpleTypeEntities;
using Teniry.CrudGenerator.SampleApi.Generators.SimpleTypeEntityGenerator;
using Moq;
using Moq.EntityFrameworkCore;

namespace Teniry.CrudGenerator.SampleApiE2eTests.HandlersTests.SimpleTypeEntityHandlersTests;

public class GetSimpleTypeEntitiesListHandlerTests {
    private readonly Mock<TestMongoDb> _db;
    private readonly GetSimpleTypeEntitiesQuery _query;
    private readonly GetSimpleTypeEntitiesHandler _sut;

    public GetSimpleTypeEntitiesListHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _query = new() {
            Ids = [new("2e83ef18-4f90-4b7d-a513-93e413bad39e")],
            Name = "Test Entity",
            Code = 'a',
            IsActive = true,
            RegistrationDateFrom = DateTime.Today.AddDays(-1),
            RegistrationDateTo = DateTime.Today.AddDays(1),
            LastSignInDateFrom = DateTime.Today.AddDays(-1),
            LastSignInDateTo = DateTime.Today.AddDays(1),
            ByteRatingFrom = 0,
            ByteRatingTo = 2,
            ShortRatingFrom = -85,
            ShortRatingTo = -80,
            IntRatingFrom = -29876718,
            IntRatingTo = -16876718,
            LongRatingFrom = -1971652637891,
            LongRatingTo = -771652637891,
            SByteRatingFrom = -7,
            SByteRatingTo = -2,
            UShortRatingFrom = 80,
            UShortRatingTo = 90,
            UIntRatingFrom = 1200000,
            UIntRatingTo = 29876718,
            ULongRatingFrom = 871652637891,
            ULongRatingTo = 1071652637891,
            FloatRatingFrom = 16.13f,
            FloatRatingTo = 20.13f,
            DoubleRatingFrom = 61873.862378,
            DoubleRatingTo = 101873.862378,
            DecimalRatingFrom = 667.97716829m,
            DecimalRatingTo = 1067.97716829m,
            NotIdGuids = [new("63c4e04c-77d3-4e27-b490-8f6e4fc635bd")],
            Sort = ["name", "code"],
            Page = 1,
            PageSize = 10
        };
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave() {
        // Arrange
        _db.Setup(x => x.Set<SimpleTypeEntity>())
            .ReturnsDbSet(
                [
                    new() {
                        Id = new("2e83ef18-4f90-4b7d-a513-93e413bad39e"),
                        Name = "Test Entity",
                        Code = 'a',
                        IsActive = true,
                        RegistrationDate = DateTime.Today,
                        LastSignInDate = DateTimeOffset.UtcNow,
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
                        NotIdGuid = new("63c4e04c-77d3-4e27-b490-8f6e4fc635bd")
                    }
                ]
            );

        // Act
        var entities = await _sut.HandleAsync(_query, new());

        // Assert
        entities.Page.Should().NotBeNull();
        entities.Page.CurrentPageIndex.Should().Be(1);
        entities.Page.PageSize.Should().Be(10);
        entities.Items.Should().SatisfyRespectively(
            dto => {
                dto.Id.Should().NotBeEmpty();
                dto.Name.Should().Be("Test Entity");
                dto.Code.Should().Be('a');
                dto.IsActive.Should().Be(true);
                dto.RegistrationDate.Should().NotBeBefore(DateTime.Today.Date.ToUniversalTime());
                dto.LastSignInDate.Should().NotBeBefore(DateTime.Today.Date.ToUniversalTime());
                dto.ByteRating.Should().BeGreaterThan(0);
                dto.ShortRating.Should().BeLessThan(0);
                dto.IntRating.Should().BeLessThan(0);
                dto.LongRating.Should().BeLessThan(0);
                dto.SByteRating.Should().BeLessThan(0);
                dto.UShortRating.Should().BeGreaterThan(0);
                dto.UIntRating.Should().BeGreaterThan(0);
                dto.ULongRating.Should().BeGreaterThan(0);
                dto.FloatRating.Should().BeGreaterThan(0);
                dto.DoubleRating.Should().BeGreaterThan(0);
                dto.DecimalRating.Should().BeGreaterThan(0);
                dto.NotIdGuid.Should().NotBeEmpty();
            }
        );
    }

    [Fact]
    public void Should_HaveCorrectSortKeys() {
        // Assert
        _query.GetSortKeys()
            .Should().ContainInConsecutiveOrder(
                "id",
                "name",
                "code",
                "isActive",
                "registrationDate",
                "lastSignInDate",
                "byteRating",
                "shortRating",
                "intRating",
                "longRating",
                "sByteRating",
                "uShortRating",
                "uIntRating",
                "uLongRating",
                "floatRating",
                "doubleRating",
                "decimalRating",
                "notIdGuid"
            );
    }

    [Theory]
    [InlineData(typeof(Guid[]), "Ids")]
    [InlineData(typeof(Guid[]), "NotIdGuids")]
    public void Filter_Should_HavePluralNames_For_ArrayProperties(Type type, string property) {
        typeof(GetSimpleTypeEntitiesFilter).Should().HaveProperty(type, property);
    }
}