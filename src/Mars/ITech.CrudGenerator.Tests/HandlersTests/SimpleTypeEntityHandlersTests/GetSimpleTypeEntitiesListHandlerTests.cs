using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.SimpleTypeEntityFeature.GetSimpleTypeEntities;
using ITech.CrudGenerator.TestApi.Generators.SimpleTypeEntityGenerator;
using Moq;
using Moq.EntityFrameworkCore;

namespace ITech.CrudGenerator.Tests.HandlersTests.SimpleTypeEntityHandlersTests;

public class GetSimpleTypeEntitiesListHandlerTests
{
    private readonly Mock<TestMongoDb> _db;
    private readonly GetSimpleTypeEntitiesQuery _query;
    private readonly GetSimpleTypeEntitiesHandler _sut;

    public GetSimpleTypeEntitiesListHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _query = new()
        {
            Page = 1,
            PageSize = 10
        };
    }

    [Fact]
    public async Task Should_ChangeEntityDataAndSave()
    {
        // Arrange
        _db.Setup(x => x.Set<SimpleTypeEntity>())
            .ReturnsDbSet([
                new SimpleTypeEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Entity",
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
                }
            ]);

        // Act
        var entities = await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        entities.Page.Should().NotBeNull();
        entities.Page.CurrentPageIndex.Should().Be(1);
        entities.Page.PageSize.Should().Be(10);
        entities.Items.Should().SatisfyRespectively(dto =>
        {
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
        });
    }
}