using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Application.CustomManagedEntityFeature.ManagedEntityCreateOperationCustomNs;
using ITech.CrudGenerator.TestApi.Generators.CustomManagedEntityGenerator;
using ITech.CrudGenerator.TestApiTests.E2eTests.Core;
using Moq;

namespace ITech.CrudGenerator.TestApiTests.HandlersTests.CustomizedManageEntityHandlersTests;

public class CreateCustomManageEntityHandlerTests {
    private readonly CustomizedNameCreateManagedEntityCommand _command;
    private readonly Mock<TestMongoDb> _db;
    private readonly CustomizedNameCreateManagedEntityHandler _sut;

    public CreateCustomManageEntityHandlerTests() {
        _db = new Mock<TestMongoDb>();
        _sut = new(_db.Object);
        _command = new() {
            Name = "My test entity"
        };
    }

    [Fact]
    public async Task Should_ReturnCorrectValue() {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<CustomManagedEntity>(), It.IsAny<CancellationToken>()))
            .Callback((CustomManagedEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        createdEntityDto.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_HasCorrectReturnModelTypeName() {
        // Arrange
        _db.Setup(x => x.AddAsync(It.IsAny<CustomManagedEntity>(), It.IsAny<CancellationToken>()))
            .Callback((CustomManagedEntity entity, CancellationToken _) => entity.Id = Guid.NewGuid());

        // Act
        var createdEntityDto = await _sut.HandleAsync(_command, new CancellationToken());

        createdEntityDto.GetType().Name.Should().Be("CustomizedNameCreatedManagedEntityDto");
    }

    [Fact]
    public async Task Should_MapCommandToEntityCorrectly() {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(
            x => x.AddAsync(
                It.Is<CustomManagedEntity>(c => c.Name.Equals("My test entity")),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Fact]
    public async Task Should_AddToDbSetAndSave() {
        // Act
        await _sut.HandleAsync(_command, new CancellationToken());

        // Assert
        _db.Verify(x => x.AddAsync(It.IsAny<CustomManagedEntity>(), It.IsAny<CancellationToken>()));
        _db.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
        _db.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("CustomizedNameCreateManagedEntityCommand")]
    [InlineData("CustomizedNameCreateManagedEntityHandler")]
    [InlineData("CustomizedNameCreatedManagedEntityDto")]
    public void Should_BeInCustomNamespace(string typeName) {
        // Assert
        typeof(Program).Assembly.Should().BeInNamespaceThatEndsWith(typeName, "ManagedEntityCreateOperationCustomNs");
    }
}