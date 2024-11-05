namespace ITech.CrudGenerator.Tests.HandlersTests.CustomGottenEntityHandlerTests;

public class CreateCustomGottenEntityHandlerTests
{
    [Theory]
    [InlineData("CreateCustomGottenEntityCommand")]
    [InlineData("CreateCustomGottenEntityHandler")]
    public void Should_NotGenerateCreateHandler(string typeName)
    {
        // Act
        var foundTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.Name.Equals(typeName));

        // Assert
        foundTypes.Should().BeEmpty();
    }
}