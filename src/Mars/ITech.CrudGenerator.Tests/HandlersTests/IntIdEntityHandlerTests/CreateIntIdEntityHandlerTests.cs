using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.IntIdEntityFeature.CreateIntIdEntity;
using ITech.CrudGenerator.TestApi.Generators.IntIdEntityGenerator;
using Moq;

namespace ITech.CrudGenerator.Tests.HandlersTests.IntIdEntityHandlerTests;

public class CreateIntIdEntityHandlerTests
{
    private readonly CreateIntIdEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CreateIntIdEntityHandler _sut;

    public CreateIntIdEntityHandlerTests()
    {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _command = new()
        {
            Name = "My test entity"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue()
    {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<IntIdEntity>(), It.IsAny<CancellationToken>()))
            .Callback((IntIdEntity entity, CancellationToken _) => entity.Id = 1);

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        createdEntityDto.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly()
    {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(
            x => x.AddAsync(It.Is<IntIdEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave()
    {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<IntIdEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }
}