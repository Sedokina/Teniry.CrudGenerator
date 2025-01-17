using Teniry.CrudGenerator.SampleApi;
using Teniry.CrudGenerator.SampleApi.Application.CustomOperationNameEntityFeature.CustomOpCreateCustomOperationNameEntity;
using Teniry.CrudGenerator.SampleApi.Generators.CustomOperationNameEntityGenerator;
using Teniry.CrudGenerator.TestApiTests.E2eTests.Core;
using Moq;

namespace Teniry.CrudGenerator.TestApiTests.HandlersTests.CustomOperationNameEntityHandlerTests;

public class CreateCustomOperationNameEntityHandlerTests {
    private readonly CustomOpCreateCustomOperationNameEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomOpCreateCustomOperationNameEntityHandler _sut;

    public CreateCustomOperationNameEntityHandlerTests() {
        _db = new();
        _sut = new(_db.Object);
        _command = new() {
            Name = "My test entity"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<CustomOperationNameEntity>(), It.IsAny<CancellationToken>()))
            .Callback((CustomOperationNameEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new());

        // Assert
        createdEntityDto.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(
            x => x.AddAsync(
                It.Is<CustomOperationNameEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave() {
        // Act
        await _sut.HandleAsync(_command, new());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<CustomOperationNameEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("CustomOpCreateCustomOperationNameEntityCommand")]
    [InlineData("CustomOpCreateCustomOperationNameEntityHandler")]
    [InlineData("CreatedCustomOperationNameEntityDto")]
    public void Should_BeInOperationNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should()
            .BeInNamespaceThatEndsWith(typeName, "CustomOpCreateCustomOperationNameEntity");
    }
}