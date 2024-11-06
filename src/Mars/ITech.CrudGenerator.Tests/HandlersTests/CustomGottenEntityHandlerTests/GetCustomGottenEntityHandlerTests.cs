using ITech.Cqrs.Domain.Exceptions;
using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomGottenEntityFeature.CustomGottenEntityGetOperationCustomNs;
using ITech.CrudGenerator.TestApi.Generators.CustomGottenEntity;
using ITech.CrudGenerator.Tests.E2eTests.Core;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests.CustomGottenEntityHandlerTests;

public class GetCustomGottenEntityHandlerTests
{
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomizedNameGetCustomEntityQuery _query;
    private readonly CustomizedNameGetCustomEntityHandler _sut;

    public GetCustomGottenEntityHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _query = new(Guid.NewGuid());
    }


    [Fact]
    public async Task Should_ThrowEntityNotFoundException_When_GettingNotExistingEntity()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<CustomGottenEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomGottenEntity?)null);

        // Act
        var act = async () => await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        await act.Should().ThrowAsync<EfEntityNotFoundException>()
            .Where(x => x.TypeName.Equals(nameof(CustomGottenEntity)));
    }

    [Fact]
    public async Task Should_GetEntityWithCorrectData()
    {
        // Arrange
        _db.Setup(x => x.FindAsync<CustomGottenEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CustomGottenEntity { Id = _query.Id, Name = "My test entity" });

        // Act
        var entity = await _sut.HandleAsync(_query, new CancellationToken());

        // Assert
        entity.Should().BeOfType<CustomizedNameGetCustomEntityDto>();
        entity.Id.Should().Be(_query.Id);
        entity.Name.Should().Be("My test entity");
        _db.Verify(x => x.FindAsync<CustomGottenEntity>(new object[] { _query.Id }, It.IsAny<CancellationToken>()),
            Times.Once);
        _db.VerifyNoOtherCalls();
    }
    
    [Theory]
    [InlineData("CustomizedNameGetCustomEntityQuery")]
    [InlineData("CustomizedNameGetCustomEntityHandler")]
    [InlineData("CustomizedNameGetCustomEntityDto")]
    public void Should_BeInCustomNamespace(string typeName)
    {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "CustomGottenEntityGetOperationCustomNs");
    }
}